import { createFileRoute, Outlet } from '@tanstack/react-router'
import ChatLayout from '@/components/chat/ChatLayout'

export const Route = createFileRoute('/chat')({
  component: ChatLayoutRoute,
})

function ChatLayoutRoute() {
  return (
    <ChatLayout>
      <Outlet />
    </ChatLayout>
  )
}
