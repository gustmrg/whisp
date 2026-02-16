import { useState } from 'react'
import { useParams } from '@tanstack/react-router'
import { Search } from 'lucide-react'
import { Input } from '@/components/ui/input'
import { ScrollArea } from '@/components/ui/scroll-area'
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar'
import { conversations, getContactById, currentUser } from '@/data/mock'
import { getInitials } from '@/lib/format'
import ConversationItem from './ConversationItem'

export default function ConversationList() {
  const [searchQuery, setSearchQuery] = useState('')
  const params = useParams({ strict: false }) as { conversationId?: string }
  const activeId = params.conversationId

  const filtered = conversations.filter((conv) => {
    if (!searchQuery.trim()) return true
    const contact = getContactById(conv.participantId)
    return contact?.name.toLowerCase().includes(searchQuery.toLowerCase())
  })

  return (
    <div className="flex h-full flex-col">
      <div className="flex shrink-0 items-center gap-3 border-b border-border px-4 py-3">
        <h1 className="text-lg font-bold text-foreground">Whisp</h1>
        <div className="flex-1" />
        <div className="relative">
          <Avatar className="h-8 w-8">
            <AvatarImage src={currentUser.avatarUrl} alt={currentUser.name} />
            <AvatarFallback className="text-xs">
              {getInitials(currentUser.name)}
            </AvatarFallback>
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
        {filtered.length === 0 ? (
          <div className="px-4 py-8 text-center text-sm text-muted-foreground">
            No conversations found
          </div>
        ) : (
          filtered.map((conv) => {
            const contact = getContactById(conv.participantId)
            if (!contact) return null
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
