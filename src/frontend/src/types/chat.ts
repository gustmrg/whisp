export enum UserStatus {
  Online = 'online',
  Offline = 'offline',
}

export enum MessageStatus {
  Sent = 'sent',
  Delivered = 'delivered',
  Read = 'read',
}

export interface User {
  id: string
  name: string
  avatarUrl: string
  status: UserStatus
  lastSeen: Date | null
}

export interface Message {
  id: string
  conversationId: string
  senderId: string
  content: string
  timestamp: Date
  status: MessageStatus
}

export interface ConversationMember {
  userId: string
  displayName: string
  role: string
}

export interface Conversation {
  id: string
  type: string
  members: ConversationMember[]
  lastMessage: Message | null
  unreadCount: number
  updatedAt: Date
}
