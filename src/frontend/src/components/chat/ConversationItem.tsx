import { Link } from '@tanstack/react-router'
import { cn } from '@/lib/utils'
import { formatConversationTimestamp } from '@/lib/format'
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar'
import { Badge } from '@/components/ui/badge'
import { getInitials } from '@/lib/format'
import { getCurrentUserId } from '@/lib/api'
import OnlineIndicator from './OnlineIndicator'
import type { Conversation, User } from '@/types/chat'

interface ConversationItemProps {
  conversation: Conversation
  contact: User
  isActive: boolean
}

export default function ConversationItem({
  conversation,
  contact,
  isActive,
}: ConversationItemProps) {
  const lastMsg = conversation.lastMessage
  const currentUserId = getCurrentUserId()
  const isOwnMessage = lastMsg?.senderId === currentUserId

  return (
    <Link
      to="/chat/$conversationId"
      params={{ conversationId: conversation.id }}
      className={cn(
        'flex items-center gap-3 px-4 py-3 transition-colors hover:bg-accent',
        isActive && 'bg-accent',
      )}
    >
      <div className="relative shrink-0">
        <Avatar className="h-12 w-12">
          <AvatarImage src={contact.avatarUrl} alt={contact.name} />
          <AvatarFallback>{getInitials(contact.name)}</AvatarFallback>
        </Avatar>
        <OnlineIndicator status={contact.status} />
      </div>
      <div className="min-w-0 flex-1">
        <div className="flex items-baseline justify-between gap-2">
          <p className="truncate text-sm font-semibold">{contact.name}</p>
          {lastMsg && (
            <span className="shrink-0 text-xs text-muted-foreground">
              {formatConversationTimestamp(lastMsg.timestamp)}
            </span>
          )}
        </div>
        <div className="flex items-center justify-between gap-2">
          <p className="truncate text-sm text-muted-foreground">
            {lastMsg
              ? `${isOwnMessage ? 'You: ' : ''}${lastMsg.content}`
              : 'No messages yet'}
          </p>
          {conversation.unreadCount > 0 && (
            <Badge className="shrink-0 rounded-full px-1.5 py-0.5 text-[10px] leading-none">
              {conversation.unreadCount}
            </Badge>
          )}
        </div>
      </div>
    </Link>
  )
}
