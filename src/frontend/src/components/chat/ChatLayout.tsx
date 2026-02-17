import { useCallback, useState } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { useParams } from '@tanstack/react-router'
import { ChatProvider } from '@/contexts/ChatContext'
import { useAppendMessage } from '@/hooks/use-messages'
import { useIsMobile } from '@/hooks/use-mobile'
import {
  useSignalR,
  type ConversationCreatedResponse,
  type MessageResponse,
} from '@/hooks/use-signalr'
import { getCurrentUserId, HUB_URL } from '@/lib/api'
import ConversationList from './ConversationList'
import UserBootstrapGate from './UserBootstrapGate'

interface ChatLayoutProps {
  children: React.ReactNode
}

export default function ChatLayout({ children }: ChatLayoutProps) {
  const queryClient = useQueryClient()
  const [currentUserId, setCurrentUserId] = useState<string | null>(() =>
    getCurrentUserId(),
  )
  const isMobile = useIsMobile()
  const params = useParams({ strict: false }) as {
    conversationId?: string
    participantId?: string
  }
  const hasActiveConversation = Boolean(params.conversationId || params.participantId)
  const hasCurrentUser = Boolean(currentUserId)

  const appendMessage = useAppendMessage()

  const handleReceiveMessage = useCallback(
    (message: MessageResponse) => {
      appendMessage(message)
      void queryClient.invalidateQueries({ queryKey: ['conversations'] })
    },
    [appendMessage, queryClient],
  )

  const handleConversationCreated = useCallback(
    (_event: ConversationCreatedResponse) => {
      void queryClient.invalidateQueries({ queryKey: ['conversations'] })
    },
    [queryClient],
  )

  const { sendMessage, sendDirectMessage, joinConversation, status } = useSignalR({
    hubUrl: HUB_URL,
    userId: currentUserId ?? '',
    enabled: hasCurrentUser,
    onReceiveMessage: handleReceiveMessage,
    onConversationCreated: handleConversationCreated,
  })

  const handleUserCreated = useCallback(() => {
    setCurrentUserId(getCurrentUserId())
  }, [])

  if (!hasCurrentUser) {
    return <UserBootstrapGate onUserCreated={handleUserCreated} />
  }

  return (
    <ChatProvider
      sendMessage={sendMessage}
      sendDirectMessage={sendDirectMessage}
      joinConversation={joinConversation}
      connectionStatus={status}
    >
      <div className="flex h-dvh w-full overflow-hidden">
        {(!isMobile || !hasActiveConversation) && (
          <aside className="w-full border-r border-border md:w-95 md:min-w-95">
            <ConversationList />
          </aside>
        )}
        {(!isMobile || hasActiveConversation) && (
          <main className="min-w-0 flex-1">{children}</main>
        )}
      </div>
    </ChatProvider>
  )
}
