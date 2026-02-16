import { cn } from '@/lib/utils'
import { UserStatus } from '@/types/chat'

interface OnlineIndicatorProps {
  status: UserStatus
  size?: 'sm' | 'md'
  className?: string
}

export default function OnlineIndicator({
  status,
  size = 'md',
  className,
}: OnlineIndicatorProps) {
  return (
    <span
      className={cn(
        'absolute bottom-0 right-0 rounded-full border-2 border-background',
        size === 'sm' ? 'h-2.5 w-2.5' : 'h-3 w-3',
        status === UserStatus.Online
          ? 'bg-online-indicator'
          : 'bg-muted-foreground',
        className,
      )}
    />
  )
}
