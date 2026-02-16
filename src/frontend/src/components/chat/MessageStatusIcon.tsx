import { Check, CheckCheck } from 'lucide-react'
import { MessageStatus } from '@/types/chat'

interface MessageStatusIconProps {
  status: MessageStatus
}

export default function MessageStatusIcon({ status }: MessageStatusIconProps) {
  switch (status) {
    case MessageStatus.Sent:
      return <Check className="h-3.5 w-3.5 text-message-outgoing-foreground/70" />
    case MessageStatus.Delivered:
      return (
        <CheckCheck className="h-3.5 w-3.5 text-message-outgoing-foreground/70" />
      )
    case MessageStatus.Read:
      return <CheckCheck className="h-3.5 w-3.5 text-blue-300" />
  }
}
