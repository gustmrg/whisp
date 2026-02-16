import { Link } from '@tanstack/react-router'
import { ArrowLeft } from 'lucide-react'
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar'
import { Button } from '@/components/ui/button'
import { useIsMobile } from '@/hooks/use-mobile'
import { formatLastSeen, getInitials } from '@/lib/format'
import { UserStatus } from '@/types/chat'
import OnlineIndicator from './OnlineIndicator'
import type { User } from '@/types/chat'

interface ChatHeaderProps {
  contact: User
}

export default function ChatHeader({ contact }: ChatHeaderProps) {
  const isMobile = useIsMobile()

  return (
    <div className="flex h-16 shrink-0 items-center gap-3 border-b border-border bg-background px-4">
      {isMobile && (
        <Button variant="ghost" size="icon" className="shrink-0" asChild>
          <Link to="/chat">
            <ArrowLeft className="h-5 w-5" />
          </Link>
        </Button>
      )}
      <div className="relative">
        <Avatar className="h-9 w-9">
          <AvatarImage src={contact.avatarUrl} alt={contact.name} />
          <AvatarFallback className="text-xs">
            {getInitials(contact.name)}
          </AvatarFallback>
        </Avatar>
        <OnlineIndicator status={contact.status} size="sm" />
      </div>
      <div className="min-w-0 flex-1">
        <p className="truncate text-sm font-semibold">{contact.name}</p>
        <p className="truncate text-xs text-muted-foreground">
          {contact.status === UserStatus.Online
            ? 'Online'
            : contact.lastSeen
              ? formatLastSeen(contact.lastSeen)
              : 'Offline'}
        </p>
      </div>
    </div>
  )
}
