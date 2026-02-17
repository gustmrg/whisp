import { useEffect, useRef } from 'react'
import { ScrollArea } from '@/components/ui/scroll-area'
import { formatDateSeparator } from '@/lib/format'
import { getCurrentUserId } from '@/lib/api'
import MessageBubble from './MessageBubble'
import type { Message } from '@/types/chat'

interface MessageListProps {
  messages: Message[]
}

export default function MessageList({ messages }: MessageListProps) {
  const bottomRef = useRef<HTMLDivElement>(null)
  const currentUserId = getCurrentUserId()

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'instant' })
  }, [messages.length])

  const groupedMessages = groupByDate(messages)

  return (
    <ScrollArea className="flex-1">
      <div className="flex flex-col gap-1 p-4">
        {groupedMessages.map((group) => (
          <div key={group.label}>
            <div className="my-4 flex items-center justify-center">
              <span className="rounded-full bg-muted px-3 py-1 text-xs text-muted-foreground">
                {group.label}
              </span>
            </div>
            <div className="flex flex-col gap-1">
              {group.messages.map((message) => (
                <MessageBubble
                  key={message.id}
                  message={message}
                  isOwn={message.senderId === currentUserId}
                />
              ))}
            </div>
          </div>
        ))}
        <div ref={bottomRef} />
      </div>
    </ScrollArea>
  )
}

interface MessageGroup {
  label: string
  messages: Message[]
}

function groupByDate(messages: Message[]): MessageGroup[] {
  const groups: MessageGroup[] = []

  for (const message of messages) {
    const label = formatDateSeparator(message.timestamp)
    const lastGroup = groups.at(-1)

    if (lastGroup && lastGroup.label === label) {
      lastGroup.messages.push(message)
    } else {
      groups.push({ label, messages: [message] })
    }
  }

  return groups
}
