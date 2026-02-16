import { MessageSquareText } from 'lucide-react'

export default function EmptyState() {
  return (
    <div className="flex h-full flex-col items-center justify-center gap-4 p-8 text-center">
      <div className="rounded-full bg-primary/10 p-6">
        <MessageSquareText className="h-12 w-12 text-primary" />
      </div>
      <div>
        <h2 className="text-xl font-semibold text-foreground">
          Welcome to Whisp
        </h2>
        <p className="mt-1 text-sm text-muted-foreground">
          Select a conversation to start messaging
        </p>
      </div>
    </div>
  )
}
