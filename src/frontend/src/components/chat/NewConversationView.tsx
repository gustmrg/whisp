import { useState, useEffect } from 'react'
import { useNavigate } from '@tanstack/react-router'
import { useQueryClient } from '@tanstack/react-query'
import { ArrowLeft, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { ScrollArea } from '@/components/ui/scroll-area'
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar'
import { getInitials } from '@/lib/format'
import { fetchConversations, fetchUsers } from '@/lib/api'
import type { ConversationResponse, UserResponse } from '@/lib/api'

interface NewConversationViewProps {
  onClose: () => void
}

export default function NewConversationView({
  onClose,
}: NewConversationViewProps) {
  const [query, setQuery] = useState('')
  const [users, setUsers] = useState<UserResponse[]>([])
  const [isLoadingUsers, setIsLoadingUsers] = useState(true)
  const [isCreating, setIsCreating] = useState(false)
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  useEffect(() => {
    let isCancelled = false

    const loadUsers = async () => {
      setIsLoadingUsers(true)
      try {
        const results = await fetchUsers()
        if (!isCancelled) {
          setUsers(results)
        }
      } catch (err) {
        console.error('Failed to load users:', err)
      } finally {
        if (!isCancelled) {
          setIsLoadingUsers(false)
        }
      }
    }

    void loadUsers()

    return () => {
      isCancelled = true
    }
  }, [])

  const normalizedQuery = query.trim().toLowerCase()
  const filteredUsers = users.filter((user) =>
    !normalizedQuery
    || user.displayName.toLowerCase().includes(normalizedQuery)
    || user.username.toLowerCase().includes(normalizedQuery),
  )

  const handleSelectUser = async (user: UserResponse) => {
    if (isCreating) return
    setIsCreating(true)

    try {
      const cachedConversations = queryClient.getQueryData<ConversationResponse[]>(['conversations'])
      const conversations =
        cachedConversations
        ?? await queryClient.fetchQuery({
          queryKey: ['conversations'],
          queryFn: fetchConversations,
        })

      const existingConversation = conversations.find(
        (conversation) =>
          conversation.type.toLowerCase() === 'direct'
          && conversation.members.some((member) => member.userId === user.id),
      )

      onClose()

      if (existingConversation) {
        navigate({
          to: '/chat/$conversationId',
          params: { conversationId: existingConversation.id },
        })
        return
      }

      navigate({
        to: '/chat/new/$participantId',
        params: { participantId: user.id },
      })
    } catch (err) {
      console.error('Failed to start direct message:', err)
    } finally {
      setIsCreating(false)
    }
  }

  return (
    <div className="flex h-full flex-col">
      <div className="flex shrink-0 items-center gap-3 border-b border-border px-4 py-3">
        <Button variant="ghost" size="icon" className="shrink-0" onClick={onClose}>
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <h2 className="text-lg font-bold text-foreground">New Chat</h2>
      </div>
      <div className="shrink-0 px-4 py-2">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="Filter contacts..."
            className="pl-9"
            autoFocus
          />
        </div>
      </div>
      <ScrollArea className="flex-1">
        {isLoadingUsers ? (
          <div className="px-4 py-8 text-center text-sm text-muted-foreground">
            Loading contacts...
          </div>
        ) : users.length === 0 ? (
          <div className="px-4 py-8 text-center text-sm text-muted-foreground">
            No contacts available
          </div>
        ) : filteredUsers.length === 0 ? (
          <div className="px-4 py-8 text-center text-sm text-muted-foreground">
            No users found
          </div>
        ) : (
          filteredUsers.map((user) => (
            <button
              key={user.id}
              onClick={() => handleSelectUser(user)}
              disabled={isCreating}
              className="flex w-full items-center gap-3 px-4 py-3 transition-colors hover:bg-accent disabled:opacity-50"
            >
              <Avatar className="h-10 w-10">
                <AvatarImage
                  src={`https://api.dicebear.com/9.x/avataaars/svg?seed=${encodeURIComponent(user.displayName)}`}
                  alt={user.displayName}
                />
                <AvatarFallback>
                  {getInitials(user.displayName)}
                </AvatarFallback>
              </Avatar>
              <div className="min-w-0 text-left">
                <p className="truncate text-sm font-semibold">
                  {user.displayName}
                </p>
                <p className="truncate text-xs text-muted-foreground">
                  @{user.username}
                </p>
              </div>
            </button>
          ))
        )}
      </ScrollArea>
    </div>
  )
}
