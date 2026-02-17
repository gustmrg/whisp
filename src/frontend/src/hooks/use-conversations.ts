import { useQuery } from '@tanstack/react-query'
import { fetchConversations } from '@/lib/api'

export function useConversations() {
  return useQuery({
    queryKey: ['conversations'],
    queryFn: fetchConversations,
    staleTime: 30_000,
  })
}
