import { createContext, useContext } from 'react'
import type { ConnectionStatus } from '@/hooks/use-signalr'

interface ChatContextValue {
  sendMessage: (conversationId: string, body: string) => Promise<void>
  sendDirectMessage: (
    participantId: string,
    body: string,
  ) => Promise<{ conversationId: string }>
  joinConversation: (conversationId: string) => Promise<void>
  connectionStatus: ConnectionStatus
}

const ChatContext = createContext<ChatContextValue | null>(null)

export function ChatProvider({
  children,
  ...value
}: ChatContextValue & { children: React.ReactNode }) {
  return <ChatContext.Provider value={value}>{children}</ChatContext.Provider>
}

export function useChatContext() {
  const ctx = useContext(ChatContext)
  if (!ctx) throw new Error('useChatContext must be used within ChatProvider')
  return ctx
}
