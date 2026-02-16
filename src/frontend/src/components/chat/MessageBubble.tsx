import { cn } from '@/lib/utils'
import { formatMessageTime } from '@/lib/format'
import MessageStatusIcon from './MessageStatusIcon'
import type { Message } from '@/types/chat'

interface MessageBubbleProps {
  message: Message
  isOwn: boolean
}

export default function MessageBubble({ message, isOwn }: MessageBubbleProps) {
  return (
    <div className={cn('flex', isOwn ? 'justify-end' : 'justify-start')}>
      <div
        className={cn(
          'max-w-[85%] px-3 py-2 md:max-w-[75%]',
          isOwn
            ? 'rounded-2xl rounded-br-sm bg-message-outgoing text-message-outgoing-foreground'
            : 'rounded-2xl rounded-bl-sm bg-message-incoming text-message-incoming-foreground',
        )}
      >
        <p className="whitespace-pre-wrap break-words text-sm">
          {message.content}
        </p>
        <div
          className={cn(
            'mt-1 flex items-center justify-end gap-1',
            isOwn ? 'text-message-outgoing-foreground/70' : 'text-muted-foreground',
          )}
        >
          <span className="text-[11px] leading-none">
            {formatMessageTime(message.timestamp)}
          </span>
          {isOwn && <MessageStatusIcon status={message.status} />}
        </div>
      </div>
    </div>
  )
}
