import { useEffect, useRef, useCallback, useState } from 'react'
import {
  HubConnectionBuilder,
  HubConnection,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr'

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

interface UseSignalROptions {
  hubUrl: string
  userId: string
  onReceiveMessage?: (message: MessageResponse) => void
  enabled?: boolean
}

export function useSignalR({
  hubUrl,
  userId,
  onReceiveMessage,
  enabled = true,
}: UseSignalROptions) {
  const connectionRef = useRef<HubConnection | null>(null)
  const onReceiveMessageRef = useRef(onReceiveMessage)
  const [status, setStatus] = useState<ConnectionStatus>('disconnected')

  onReceiveMessageRef.current = onReceiveMessage

  useEffect(() => {
    if (!enabled || !userId) return

    const connection = new HubConnectionBuilder()
      .withUrl(`${hubUrl}?userId=${userId}`)
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(LogLevel.Information)
      .build()

    connectionRef.current = connection

    connection.on('ReceiveMessage', (message: MessageResponse) => {
      onReceiveMessageRef.current?.(message)
    })

    connection.onreconnecting(() => setStatus('reconnecting'))
    connection.onreconnected(() => setStatus('connected'))
    connection.onclose(() => setStatus('disconnected'))

    setStatus('connecting')
    connection.start().then(
      () => setStatus('connected'),
      (err) => {
        console.error('SignalR connection failed:', err)
        setStatus('disconnected')
      },
    )

    return () => {
      connection.stop()
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

  return { status, sendMessage, joinConversation }
}
