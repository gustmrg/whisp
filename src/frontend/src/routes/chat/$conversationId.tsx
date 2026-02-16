import { createFileRoute } from '@tanstack/react-router'
import ChatView from '@/components/chat/ChatView'

export const Route = createFileRoute('/chat/$conversationId')({
  component: ConversationRoute,
})

function ConversationRoute() {
  const { conversationId } = Route.useParams()
  return <ChatView key={conversationId} conversationId={conversationId} />
}
