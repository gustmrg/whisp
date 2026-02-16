import { useState } from 'react'
import {
  conversations,
  getContactById,
  getMessagesForConversation,
  currentUser,
} from '@/data/mock'
import { MessageStatus } from '@/types/chat'
import ChatHeader from './ChatHeader'
import MessageList from './MessageList'
import MessageInput from './MessageInput'
import type { Message } from '@/types/chat'

interface ChatViewProps {
  conversationId: string
}

export default function ChatView({ conversationId }: ChatViewProps) {
  const conversation = conversations.find((c) => c.id === conversationId)
  const contact = conversation
    ? getContactById(conversation.participantId)
    : undefined
  const [messages, setMessages] = useState<Message[]>(() =>
    getMessagesForConversation(conversationId),
  )

  if (!contact) {
    return (
      <div className="flex h-full items-center justify-center text-muted-foreground">
        Conversation not found
      </div>
    )
  }

  const handleSendMessage = (content: string) => {
    const newMessage: Message = {
      id: `msg-new-${Date.now()}`,
      conversationId,
      senderId: currentUser.id,
      content,
      timestamp: new Date(),
      status: MessageStatus.Sent,
    }
    setMessages((prev) => [...prev, newMessage])
  }

  return (
    <div className="flex h-full flex-col">
      <ChatHeader contact={contact} />
      <MessageList messages={messages} />
      <MessageInput onSendMessage={handleSendMessage} />
    </div>
  )
}
