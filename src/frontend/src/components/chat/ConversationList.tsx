import { useState } from 'react'
import { useParams } from '@tanstack/react-router'
import { Search, SquarePen } from 'lucide-react'
import { Input } from '@/components/ui/input'
import { ScrollArea } from '@/components/ui/scroll-area'
import { Avatar, AvatarFallback } from '@/components/ui/avatar'
import { Button } from '@/components/ui/button'
import { useConversations } from '@/hooks/use-conversations'
import {
  mapConversationResponse,
  getContactFromConversation,
} from '@/lib/api'
import ConversationItem from './ConversationItem'
import NewConversationView from './NewConversationView'

export default function ConversationList() {
  const [searchQuery, setSearchQuery] = useState('')
  const [isCreating, setIsCreating] = useState(false)
  const params = useParams({ strict: false }) as { conversationId?: string }
  const activeId = params.conversationId

  const { data: rawConversations, isLoading } = useConversations()
  const conversations = (rawConversations ?? []).map(mapConversationResponse)

  const filtered = conversations.filter((conv) => {
    if (!searchQuery.trim()) return true
    const contact = getContactFromConversation(conv)
    return contact.name.toLowerCase().includes(searchQuery.toLowerCase())
  })

  if (isCreating) {
    return <NewConversationView onClose={() => setIsCreating(false)} />
  }

  return (
    <div className="flex h-full flex-col">
      <div className="flex shrink-0 items-center gap-3 border-b border-border px-4 py-3">
        <h1 className="text-lg font-bold text-foreground">Whisp</h1>
        <div className="flex-1" />
        <Button
          variant="ghost"
          size="icon"
          className="shrink-0"
          onClick={() => setIsCreating(true)}
        >
          <SquarePen className="h-5 w-5" />
        </Button>
        <div className="relative">
          <Avatar className="h-8 w-8">
            <AvatarFallback className="text-xs">You</AvatarFallback>
          </Avatar>
        </div>
      </div>
      <div className="shrink-0 px-4 py-2">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search conversations..."
            className="pl-9"
          />
        </div>
      </div>
      <ScrollArea className="flex-1">
        {isLoading ? (
          <div className="px-4 py-8 text-center text-sm text-muted-foreground">
            Loading...
          </div>
        ) : filtered.length === 0 ? (
          <div className="px-4 py-8 text-center text-sm text-muted-foreground">
            No conversations found
          </div>
        ) : (
          filtered.map((conv) => {
            const contact = getContactFromConversation(conv)
            return (
              <ConversationItem
                key={conv.id}
                conversation={conv}
                contact={contact}
                isActive={activeId === conv.id}
              />
            )
          })
        )}
      </ScrollArea>
    </div>
  )
}
