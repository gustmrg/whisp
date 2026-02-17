import { useChatContext } from '@/contexts/ChatContext'
import { useMessages } from '@/hooks/use-messages'
import { useConversations } from '@/hooks/use-conversations'
import {
  mapMessageResponse,
  mapConversationResponse,
  getContactFromConversation,
} from '@/lib/api'
import ChatHeader from './ChatHeader'
import MessageList from './MessageList'
import MessageInput from './MessageInput'

interface ChatViewProps {
  conversationId: string
}

export default function ChatView({ conversationId }: ChatViewProps) {
  const { data: rawMessages, isLoading } = useMessages(conversationId)
  const { data: rawConversations } = useConversations()
  const { sendMessage } = useChatContext()

  const messages = (rawMessages ?? []).map(mapMessageResponse)

  const conversation = rawConversations
    ?.map(mapConversationResponse)
    .find((c) => c.id === conversationId)
  const contact = conversation
    ? getContactFromConversation(conversation)
    : undefined

  const handleSendMessage = async (content: string) => {
    await sendMessage(conversationId, content)
  }

  if (isLoading) {
    return (
      <div className="flex h-full items-center justify-center text-muted-foreground">
        Loading messages...
      </div>
    )
  }

  if (!contact) {
    return (
      <div className="flex h-full items-center justify-center text-muted-foreground">
        Conversation not found
      </div>
    )
  }

  return (
    <div className="flex h-full flex-col">
      <ChatHeader contact={contact} />
      <MessageList messages={messages} />
      <MessageInput onSendMessage={handleSendMessage} />
    </div>
  )
}
