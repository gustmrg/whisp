import { useParams } from '@tanstack/react-router'
import { useIsMobile } from '@/hooks/use-mobile'
import ConversationList from './ConversationList'

interface ChatLayoutProps {
  children: React.ReactNode
}

export default function ChatLayout({ children }: ChatLayoutProps) {
  const isMobile = useIsMobile()
  const params = useParams({ strict: false }) as { conversationId?: string }
  const hasActiveConversation = Boolean(params.conversationId)

  return (
    <div className="flex h-dvh w-full overflow-hidden">
      {(!isMobile || !hasActiveConversation) && (
        <aside className="w-full border-r border-border md:w-[380px] md:min-w-[380px]">
          <ConversationList />
        </aside>
      )}
      {(!isMobile || hasActiveConversation) && (
        <main className="min-w-0 flex-1">{children}</main>
      )}
    </div>
  )
}
