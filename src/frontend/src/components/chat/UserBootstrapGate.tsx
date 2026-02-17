import { useState, type FormEvent } from 'react'
import { Loader2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { useIsMobile } from '@/hooks/use-mobile'
import { createUser, setCurrentUserId } from '@/lib/api'
import { cn } from '@/lib/utils'

interface UserBootstrapGateProps {
  onUserCreated: () => void
}

export default function UserBootstrapGate({
  onUserCreated,
}: UserBootstrapGateProps) {
  const isMobile = useIsMobile()
  const [username, setUsername] = useState('')
  const [displayName, setDisplayName] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    const normalizedUsername = username.trim()
    const normalizedDisplayName = displayName.trim()

    if (!normalizedUsername) {
      setErrorMessage('Username is required')
      return
    }

    if (!normalizedDisplayName) {
      setErrorMessage('Display name is required')
      return
    }

    setIsSubmitting(true)
    setErrorMessage(null)

    try {
      const user = await createUser(normalizedUsername, normalizedDisplayName)
      setCurrentUserId(user.id)
      onUserCreated()
    } catch (error) {
      const status =
        typeof error === 'object'
        && error
        && 'status' in error
        && typeof error.status === 'number'
          ? error.status
          : undefined

      if (status === 409) {
        setErrorMessage('Username is already taken')
      } else if (status === 400) {
        setErrorMessage('Please provide a valid username and display name')
      } else {
        setErrorMessage('Failed to create user. Please try again.')
      }
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-end justify-center bg-black/45 p-4 md:items-center">
      <section
        className={cn(
          'w-full border border-border bg-background shadow-xl',
          isMobile
            ? 'max-w-none rounded-t-2xl px-4 pb-[calc(1rem+env(safe-area-inset-bottom))] pt-5'
            : 'max-w-md rounded-2xl p-6',
        )}
      >
        <h1 className="text-xl font-bold text-foreground">Set up your test user</h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Create a temporary profile to start testing conversations.
        </p>

        <form onSubmit={handleSubmit} className="mt-5 space-y-4">
          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground" htmlFor="username">
              Username
            </label>
            <Input
              id="username"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
              placeholder="e.g. qa_alice"
              autoComplete="off"
              maxLength={30}
              disabled={isSubmitting}
              autoFocus
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-foreground" htmlFor="displayName">
              Display name
            </label>
            <Input
              id="displayName"
              value={displayName}
              onChange={(event) => setDisplayName(event.target.value)}
              placeholder="e.g. Alice QA"
              maxLength={50}
              disabled={isSubmitting}
            />
          </div>

          {errorMessage && (
            <p className="text-sm text-destructive" role="alert">
              {errorMessage}
            </p>
          )}

          <Button type="submit" className="w-full" disabled={isSubmitting}>
            {isSubmitting && <Loader2 className="h-4 w-4 animate-spin" />}
            Continue to chat
          </Button>
        </form>
      </section>
    </div>
  )
}
