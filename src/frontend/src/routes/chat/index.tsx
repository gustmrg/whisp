import { createFileRoute } from '@tanstack/react-router'
import EmptyState from '@/components/chat/EmptyState'

export const Route = createFileRoute('/chat/')({
  component: ChatIndex,
})

function ChatIndex() {
  return <EmptyState />
}
