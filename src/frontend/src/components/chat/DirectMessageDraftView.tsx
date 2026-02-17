import { useEffect, useMemo, useState } from 'react'
import { useNavigate } from '@tanstack/react-router'
import { useQueryClient } from '@tanstack/react-query'
import { fetchUsers } from '@/lib/api'
import { UserStatus, type User } from '@/types/chat'
import { useChatContext } from '@/contexts/ChatContext'
import MessageInput from './MessageInput'
import ChatHeader from './ChatHeader'

interface DirectMessageDraftViewProps {
  participantId: string
}

export default function DirectMessageDraftView({
  participantId,
}: DirectMessageDraftViewProps) {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const { sendDirectMessage } = useChatContext()
  const [isSending, setIsSending] = useState(false)
  const [participantName, setParticipantName] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false

    const loadParticipant = async () => {
      try {
        const users = await fetchUsers()
        if (cancelled) return

        const participant = users.find((user) => user.id === participantId)
        setParticipantName(participant?.displayName ?? null)
      } catch (error) {
        if (!cancelled) {
          console.error('Failed to load participant details:', error)
        }
      }
    }

    void loadParticipant()

    return () => {
      cancelled = true
    }
  }, [participantId])

  const contact = useMemo<User>(
    () => ({
      id: participantId,
      name: participantName ?? 'New Chat',
      avatarUrl: `https://api.dicebear.com/9.x/avataaars/svg?seed=${encodeURIComponent(participantName ?? participantId)}`,
      status: UserStatus.Offline,
      lastSeen: null,
    }),
    [participantId, participantName],
  )

  const handleSendMessage = async (content: string) => {
    if (isSending) return

    setIsSending(true)
    try {
      const response = await sendDirectMessage(participantId, content)
      await queryClient.invalidateQueries({ queryKey: ['conversations'] })
      navigate({
        to: '/chat/$conversationId',
        params: { conversationId: response.conversationId },
      })
    } catch (error) {
      console.error('Failed to send first direct message:', error)
    } finally {
      setIsSending(false)
    }
  }

  return (
    <div className="flex h-full flex-col">
      <ChatHeader contact={contact} />
      <div className="flex flex-1 items-center justify-center px-6 text-center text-sm text-muted-foreground">
        Send a message to start this conversation.
      </div>
      <MessageInput onSendMessage={handleSendMessage} />
    </div>
  )
}
