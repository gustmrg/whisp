import type { MessageResponse } from '@/hooks/use-signalr'

const API_BASE =
  import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5265'

// Temporary hardcoded user ID — replace with auth when available
export const CURRENT_USER_ID = '00000000-0000-0000-0000-000000000001'

export const HUB_URL = `${API_BASE}/hubs/chat`

export async function fetchMessages(
  conversationId: string,
  skip = 0,
  take = 50,
): Promise<MessageResponse[]> {
  const params = new URLSearchParams({
    userId: CURRENT_USER_ID,
    skip: skip.toString(),
    take: take.toString(),
  })
  const res = await fetch(
    `${API_BASE}/api/conversations/${conversationId}/messages?${params}`,
  )
  if (!res.ok) throw new Error(`Failed to fetch messages: ${res.status}`)
  const messages: MessageResponse[] = await res.json()
  // API returns newest-first; reverse for chronological display
  return messages.reverse()
}

export interface ConversationResponse {
  id: string
  type: string
  createdAt: string
  members: MemberResponse[]
  lastMessage: MessageResponse | null
}

export interface MemberResponse {
  userId: string
  displayName: string
  role: string
}

export async function fetchConversations(): Promise<ConversationResponse[]> {
  const params = new URLSearchParams({ userId: CURRENT_USER_ID })
  const res = await fetch(`${API_BASE}/api/conversations?${params}`)
  if (!res.ok)
    throw new Error(`Failed to fetch conversations: ${res.status}`)
  return res.json()
}

export interface UserResponse {
  id: string
  username: string
  displayName: string
}

export async function searchUsers(query: string): Promise<UserResponse[]> {
  const params = new URLSearchParams({ q: query, userId: CURRENT_USER_ID })
  const res = await fetch(`${API_BASE}/api/users/search?${params}`)
  if (!res.ok) throw new Error(`Failed to search users: ${res.status}`)
  return res.json()
}

export async function createConversation(
  participantId: string,
): Promise<ConversationResponse> {
  const params = new URLSearchParams({ userId: CURRENT_USER_ID })
  const res = await fetch(`${API_BASE}/api/conversations?${params}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ participantId }),
  })
  if (!res.ok)
    throw new Error(`Failed to create conversation: ${res.status}`)
  return res.json()
}

// --- Mappers ---

import type { Message, Conversation, User } from '@/types/chat'
import { MessageStatus, UserStatus } from '@/types/chat'

export function mapMessageResponse(r: MessageResponse): Message {
  return {
    id: r.id,
    conversationId: r.conversationId,
    senderId: r.senderId,
    content: r.body,
    timestamp: new Date(r.createdAt),
    status: MessageStatus.Sent,
  }
}

export function mapConversationResponse(r: ConversationResponse): Conversation {
  return {
    id: r.id,
    type: r.type,
    members: r.members,
    lastMessage: r.lastMessage ? mapMessageResponse(r.lastMessage) : null,
    unreadCount: 0,
    updatedAt: new Date(r.createdAt),
  }
}

export function getContactFromConversation(conv: Conversation): User {
  const other = conv.members.find((m) => m.userId !== CURRENT_USER_ID)
  const name = other?.displayName ?? 'Unknown'
  return {
    id: other?.userId ?? '',
    name,
    avatarUrl: `https://api.dicebear.com/9.x/avataaars/svg?seed=${encodeURIComponent(name)}`,
    status: UserStatus.Offline,
    lastSeen: null,
  }
}
