import { useState, useEffect } from 'react'
import { useNavigate } from '@tanstack/react-router'
import { useQueryClient } from '@tanstack/react-query'
import { ArrowLeft, Search } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { ScrollArea } from '@/components/ui/scroll-area'
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar'
import { getInitials } from '@/lib/format'
import { searchUsers, createConversation } from '@/lib/api'
import type { UserResponse } from '@/lib/api'
import { useChatContext } from '@/contexts/ChatContext'

interface NewConversationViewProps {
  onClose: () => void
}

export default function NewConversationView({
  onClose,
}: NewConversationViewProps) {
  const [query, setQuery] = useState('')
  const [users, setUsers] = useState<UserResponse[]>([])
  const [isSearching, setIsSearching] = useState(false)
  const [isCreating, setIsCreating] = useState(false)
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const { joinConversation } = useChatContext()

  useEffect(() => {
    if (!query.trim()) {
      setUsers([])
      return
    }

    const timeout = setTimeout(async () => {
      setIsSearching(true)
      try {
        const results = await searchUsers(query.trim())
        setUsers(results)
      } catch (err) {
        console.error('Failed to search users:', err)
      } finally {
        setIsSearching(false)
      }
    }, 300)

    return () => clearTimeout(timeout)
  }, [query])

  const handleSelectUser = async (user: UserResponse) => {
    if (isCreating) return
    setIsCreating(true)

    try {
      const conversation = await createConversation(user.id)
      await joinConversation(conversation.id)
      await queryClient.invalidateQueries({ queryKey: ['conversations'] })
      onClose()
      navigate({ to: '/chat/$conversationId', params: { conversationId: conversation.id } })
    } catch (err) {
      console.error('Failed to create conversation:', err)
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
            placeholder="Search by username..."
            className="pl-9"
            autoFocus
          />
        </div>
      </div>
      <ScrollArea className="flex-1">
        {isSearching ? (
          <div className="px-4 py-8 text-center text-sm text-muted-foreground">
            Searching...
          </div>
        ) : !query.trim() ? (
          <div className="px-4 py-8 text-center text-sm text-muted-foreground">
            Search for a user to start a conversation
          </div>
        ) : users.length === 0 ? (
          <div className="px-4 py-8 text-center text-sm text-muted-foreground">
            No users found
          </div>
        ) : (
          users.map((user) => (
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
