import { ChatProvider } from '@/contexts/ChatContext'
import { useAppendMessage } from '@/hooks/use-messages'
import { useIsMobile } from '@/hooks/use-mobile'
import { useSignalR } from '@/hooks/use-signalr'
import { CURRENT_USER_ID, HUB_URL } from '@/lib/api'
import { useParams } from '@tanstack/react-router'
import ConversationList from './ConversationList'

interface ChatLayoutProps {
  children: React.ReactNode
}

export default function ChatLayout({ children }: ChatLayoutProps) {
  const isMobile = useIsMobile()
  const params = useParams({ strict: false }) as { conversationId?: string }
  const hasActiveConversation = Boolean(params.conversationId)

  const appendMessage = useAppendMessage()
  const { sendMessage, joinConversation, status } = useSignalR({
    hubUrl: HUB_URL,
    userId: CURRENT_USER_ID,
    onReceiveMessage: appendMessage,
  })

  return (
    <ChatProvider sendMessage={sendMessage} joinConversation={joinConversation} connectionStatus={status}>
      <div className="flex h-dvh w-full overflow-hidden">
        {(!isMobile || !hasActiveConversation) && (
          <aside className="w-full border-r border-border md:w-95 md:min-w-95">
            <ConversationList />
          </aside>
        )}
        {(!isMobile || hasActiveConversation) && (
          <main className="min-w-0 flex-1">{children}</main>
        )}
      </div>
    </ChatProvider>
  )
}
