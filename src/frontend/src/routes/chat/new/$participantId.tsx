import { createFileRoute } from '@tanstack/react-router'
import DirectMessageDraftView from '@/components/chat/DirectMessageDraftView'

export const Route = createFileRoute('/chat/new/$participantId')({
  component: NewDirectMessageRoute,
})

function NewDirectMessageRoute() {
  const { participantId } = Route.useParams()
  return <DirectMessageDraftView participantId={participantId} />
}
