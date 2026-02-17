import type { MessageResponse } from '@/hooks/use-signalr'
import { MessageStatus, UserStatus } from '@/types/chat'
import type { Conversation, Message, User } from '@/types/chat'

const API_BASE =
  import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5265'

const CURRENT_USER_STORAGE_KEY = 'whisp.currentUserId'

function requireCurrentUserId(): string {
  const userId = getCurrentUserId()
  if (!userId) {
    throw new Error('Current user is not set')
  }
  return userId
}

export function getCurrentUserId(): string | null {
  if (typeof window === 'undefined') return null
  const userId = window.localStorage.getItem(CURRENT_USER_STORAGE_KEY)?.trim()
  return userId || null
}

export function setCurrentUserId(userId: string) {
  if (typeof window === 'undefined') return
  window.localStorage.setItem(CURRENT_USER_STORAGE_KEY, userId)
}

export function clearCurrentUserId() {
  if (typeof window === 'undefined') return
  window.localStorage.removeItem(CURRENT_USER_STORAGE_KEY)
}

export const HUB_URL = `${API_BASE}/hubs/chat`

export async function fetchMessages(
  conversationId: string,
  skip = 0,
  take = 50,
): Promise<MessageResponse[]> {
  const userId = requireCurrentUserId()
  const params = new URLSearchParams({
    userId,
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
  const userId = requireCurrentUserId()
  const params = new URLSearchParams({ userId })
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

export async function createUser(
  username: string,
  displayName: string,
): Promise<UserResponse> {
  const res = await fetch(`${API_BASE}/api/users`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, displayName }),
  })

  if (!res.ok) {
    const error = new Error(`Failed to create user: ${res.status}`) as Error & {
      status: number
    }
    error.status = res.status
    throw error
  }

  return res.json()
}

export async function fetchUsers(): Promise<UserResponse[]> {
  const userId = requireCurrentUserId()
  const params = new URLSearchParams({ userId })
  const res = await fetch(`${API_BASE}/api/users?${params}`)
  if (!res.ok) throw new Error(`Failed to fetch users: ${res.status}`)
  return res.json()
}

export async function searchUsers(query: string): Promise<UserResponse[]> {
  const userId = requireCurrentUserId()
  const params = new URLSearchParams({ q: query, userId })
  const res = await fetch(`${API_BASE}/api/users/search?${params}`)
  if (!res.ok) throw new Error(`Failed to search users: ${res.status}`)
  return res.json()
}

export async function createConversation(
  participantId: string,
): Promise<ConversationResponse> {
  const userId = requireCurrentUserId()
  const params = new URLSearchParams({ userId })
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
  const currentUserId = getCurrentUserId()
  const other = currentUserId
    ? conv.members.find((m) => m.userId !== currentUserId)
    : conv.members.at(0)
  const name = other?.displayName ?? 'Unknown'
  return {
    id: other?.userId ?? '',
    name,
    avatarUrl: `https://api.dicebear.com/9.x/avataaars/svg?seed=${encodeURIComponent(name)}`,
    status: UserStatus.Offline,
    lastSeen: null,
  }
}
