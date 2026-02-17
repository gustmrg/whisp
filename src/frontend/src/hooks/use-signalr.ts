import { useEffect, useRef, useCallback, useState } from 'react'
import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr'
import type { HubConnection } from '@microsoft/signalr'

export type ConnectionStatus =
  | 'disconnected'
  | 'connecting'
  | 'connected'
  | 'reconnecting'

export interface MessageResponse {
  id: string
  conversationId: string
  senderId: string
  senderDisplayName: string
  body: string
  createdAt: string
  sentAt: string | null
}

export interface ConversationCreatedResponse {
  conversationId: string
}

export interface SendDirectMessageResponse {
  conversationId: string
  message: MessageResponse
  conversationCreated: boolean
}

interface UseSignalROptions {
  hubUrl: string
  userId: string
  onReceiveMessage?: (message: MessageResponse) => void
  onConversationCreated?: (event: ConversationCreatedResponse) => void
  enabled?: boolean
}

export function useSignalR({
  hubUrl,
  userId,
  onReceiveMessage,
  onConversationCreated,
  enabled = true,
}: UseSignalROptions) {
  const connectionRef = useRef<HubConnection | null>(null)
  const onReceiveMessageRef = useRef(onReceiveMessage)
  const onConversationCreatedRef = useRef(onConversationCreated)
  const [status, setStatus] = useState<ConnectionStatus>('disconnected')

  onReceiveMessageRef.current = onReceiveMessage
  onConversationCreatedRef.current = onConversationCreated

  useEffect(() => {
    if (!enabled || !userId) return

    let isDisposed = false
    let retryTimer: ReturnType<typeof setTimeout> | null = null

    const connection = new HubConnectionBuilder()
      .withUrl(`${hubUrl}?userId=${userId}`)
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(LogLevel.Information)
      .build()

    connectionRef.current = connection

    connection.on('ReceiveMessage', (message: MessageResponse) => {
      onReceiveMessageRef.current?.(message)
    })

    connection.on('ConversationCreated', (event: ConversationCreatedResponse) => {
      onConversationCreatedRef.current?.(event)
    })

    connection.onreconnecting(() => setStatus('reconnecting'))
    connection.onreconnected(() => setStatus('connected'))
    connection.onclose(() => setStatus('disconnected'))

    const startConnection = async () => {
      if (isDisposed) return

      setStatus('connecting')
      try {
        await connection.start()
        if (!isDisposed) {
          setStatus('connected')
        }
      } catch (err) {
        if (isDisposed) return

        console.error('SignalR connection failed:', err)
        setStatus('disconnected')
        retryTimer = setTimeout(() => {
          void startConnection()
        }, 2000)
      }
    }

    void startConnection()

    return () => {
      isDisposed = true
      if (retryTimer) {
        clearTimeout(retryTimer)
      }
      void connection.stop()
      connectionRef.current = null
    }
  }, [hubUrl, userId, enabled])

  const sendMessage = useCallback(
    async (conversationId: string, body: string) => {
      const connection = connectionRef.current
      if (!connection || connection.state !== HubConnectionState.Connected) {
        throw new Error('Not connected to chat hub')
      }
      await connection.invoke('SendMessage', { conversationId, body })
    },
    [],
  )

  const joinConversation = useCallback(
    async (conversationId: string) => {
      const connection = connectionRef.current
      if (!connection || connection.state !== HubConnectionState.Connected) {
        throw new Error('Not connected to chat hub')
      }
      await connection.invoke('JoinConversation', conversationId)
    },
    [],
  )

  const sendDirectMessage = useCallback(
    async (participantId: string, body: string) => {
      const connection = connectionRef.current
      if (!connection || connection.state !== HubConnectionState.Connected) {
        throw new Error('Not connected to chat hub')
      }

      return connection.invoke<SendDirectMessageResponse>('SendDirectMessage', {
        participantId,
        body,
      })
    },
    [],
  )

  return { status, sendMessage, sendDirectMessage, joinConversation }
}
