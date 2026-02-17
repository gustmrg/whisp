import { useQuery, useQueryClient } from '@tanstack/react-query'
import { fetchMessages } from '@/lib/api'
import type { MessageResponse } from '@/hooks/use-signalr'

export function useMessages(conversationId: string) {
  return useQuery({
    queryKey: ['messages', conversationId],
    queryFn: () => fetchMessages(conversationId),
    staleTime: Infinity,
  })
}

export function useAppendMessage() {
  const queryClient = useQueryClient()

  return (message: MessageResponse) => {
    queryClient.setQueryData<MessageResponse[]>(
      ['messages', message.conversationId],
      (old) => {
        if (!old) return [message]
        if (old.some((m) => m.id === message.id)) return old
        return [...old, message]
      },
    )
  }
}
