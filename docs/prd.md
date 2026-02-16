# Product Requirements Document (PRD)

## ChatApp — Real-Time Text Chat Application

| Field             | Detail                     |
| ----------------- | -------------------------- |
| **Author**        | —                          |
| **Created**       | February 16, 2026          |
| **Last Updated**  | February 16, 2026          |
| **Status**        | Draft                      |
| **Version**       | 1.0                        |

---

## 1. Overview

ChatApp is a real-time text chat application designed as a personal engineering project with enterprise-grade architecture and technology choices. The primary objective is skill development across modern backend and frontend technologies while building a fully functional, production-quality messaging platform.

The application supports direct messaging between two users, group conversations with multiple participants, and channel-based communication. It leverages WebSocket connections for instant message delivery, a relational database for persistent storage, and an in-memory cache for high-frequency ephemeral data such as online presence and recent messages.

---

## 2. Goals and Objectives

### 2.1 Project Goals

- Build a fully functional real-time chat application from the ground up.
- Apply enterprise-level architectural patterns including Clean Architecture, CQRS, and domain-driven design principles.
- Gain hands-on experience with .NET 10, SignalR, PostgreSQL, Redis, and React.
- Develop a system that is horizontally scalable by design, even if initially deployed as a single instance.
- Produce a portfolio-quality project that demonstrates backend engineering depth.

### 2.2 Non-Goals

- This is not a commercial product and will not be launched to the public.
- Mobile native applications (iOS/Android) are out of scope; the frontend is web-only.
- End-to-end encryption is not in scope for the initial version.
- Voice and video calling are excluded.
- Modular monolith architecture is intentionally excluded as the domain does not warrant bounded context separation.

---

## 3. Target Users

Since this is a personal learning project, the primary user is the developer and a small group of testers. However, the application is designed as if it were targeting the following user personas to ensure realistic architecture decisions.

**Persona 1 — Individual User:** A person who wants to message friends or colleagues in real time through direct chats and group conversations.

**Persona 2 — Team Lead or Community Manager:** A person who creates and manages group conversations or channels, assigns roles, and moderates participation.

---

## 4. Functional Requirements

### 4.1 Authentication and User Management

| ID     | Requirement                                                                                         | Priority |
| ------ | --------------------------------------------------------------------------------------------------- | -------- |
| AUTH-1 | Users can register with a unique username, email, and password.                                     | P0       |
| AUTH-2 | Users can log in and receive a JWT access token and a refresh token.                                | P0       |
| AUTH-3 | Access tokens expire after a configurable duration (default 15 minutes) and can be refreshed.       | P0       |
| AUTH-4 | Users can log out, which invalidates the refresh token stored in Redis.                             | P0       |
| AUTH-5 | Users can update their profile including display name, avatar URL, and status message.              | P1       |
| AUTH-6 | Passwords are hashed using a strong algorithm (bcrypt or Argon2) before storage.                    | P0       |
| AUTH-7 | Users can soft-delete their account, which anonymizes their data but preserves message history.     | P2       |

### 4.2 Conversations

| ID      | Requirement                                                                                                  | Priority |
| ------- | ------------------------------------------------------------------------------------------------------------ | -------- |
| CONV-1  | Users can start a direct conversation with another user. Only one direct conversation can exist per user pair.| P0       |
| CONV-2  | Users can create a group conversation with a name, optional description, and two or more members.            | P0       |
| CONV-3  | Users can create a channel (open conversation) that other users can discover and join.                       | P1       |
| CONV-4  | Group and channel conversations display a name and optional avatar.                                          | P1       |
| CONV-5  | Conversation creators are assigned the "owner" role. Owners can promote members to "admin".                  | P1       |
| CONV-6  | Admins and owners can add or remove members from group conversations.                                        | P1       |
| CONV-7  | Members can leave a conversation voluntarily.                                                                | P0       |
| CONV-8  | Users can mute notifications for a specific conversation.                                                    | P2       |
| CONV-9  | The conversation list displays the most recent message preview, timestamp, and unread count for each entry.  | P0       |
| CONV-10 | Conversations are sorted by the most recent activity (last message timestamp).                                | P0       |

### 4.3 Messaging

| ID     | Requirement                                                                                           | Priority |
| ------ | ----------------------------------------------------------------------------------------------------- | -------- |
| MSG-1  | Users can send text messages in real time to any conversation they are a member of.                   | P0       |
| MSG-2  | Messages are delivered instantly to all online members via WebSocket (SignalR).                        | P0       |
| MSG-3  | Messages are persisted to PostgreSQL and cached in Redis for fast retrieval.                          | P0       |
| MSG-4  | Users can load message history with cursor-based pagination (newest first, load older on scroll up).  | P0       |
| MSG-5  | Users can reply to a specific message, creating a visible parent-child thread link.                   | P1       |
| MSG-6  | Users can edit their own messages. Edited messages display an "edited" indicator.                     | P1       |
| MSG-7  | Users can soft-delete their own messages. Deleted messages display a "message was deleted" placeholder.| P1       |
| MSG-8  | Users can react to messages with emoji. Multiple users can add the same reaction.                     | P2       |
| MSG-9  | Messages support different types: text, image, file, and system (e.g., "User X joined the group").   | P1       |
| MSG-10 | System messages are generated automatically for events like member join, member leave, and role change.| P1       |

### 4.4 File Sharing

| ID     | Requirement                                                                                    | Priority |
| ------ | ---------------------------------------------------------------------------------------------- | -------- |
| FILE-1 | Users can upload and share images and files within a conversation.                             | P1       |
| FILE-2 | Uploaded files are stored in an external object store (e.g., AWS S3 or MinIO for local dev).   | P1       |
| FILE-3 | Image attachments display an inline preview in the chat. Other files display as download links. | P1       |
| FILE-4 | File uploads are limited to a configurable maximum size (default 25 MB).                       | P1       |

### 4.5 Presence and Typing Indicators

| ID     | Requirement                                                                                              | Priority |
| ------ | -------------------------------------------------------------------------------------------------------- | -------- |
| PRES-1 | Online/offline status is tracked in real time via SignalR connection events and stored in Redis.          | P0       |
| PRES-2 | Users see a visual indicator (green dot or similar) showing which contacts are currently online.          | P0       |
| PRES-3 | When a user disconnects, their status updates to "offline" after a short grace period (e.g., 30 seconds).| P1       |
| PRES-4 | A "last seen" timestamp is recorded when a user goes offline.                                            | P1       |
| PRES-5 | Typing indicators ("User is typing...") are broadcast to conversation members via SignalR.               | P1       |
| PRES-6 | Typing indicators automatically expire after a timeout (e.g., 3 seconds of inactivity).                 | P1       |

### 4.6 Notifications and Unread Tracking

| ID      | Requirement                                                                                                     | Priority |
| ------- | --------------------------------------------------------------------------------------------------------------- | -------- |
| NOTIF-1 | Each conversation tracks a per-user unread message count, stored as an atomic counter in Redis.                 | P0       |
| NOTIF-2 | When a user opens a conversation, the unread count resets to zero and `last_read_at` is updated in the database.| P0       |
| NOTIF-3 | The conversation list sidebar displays the unread count badge for each conversation.                            | P0       |
| NOTIF-4 | Browser push notifications are sent for new messages when the application tab is not focused.                   | P2       |

### 4.7 Search

| ID      | Requirement                                                                                   | Priority |
| ------- | --------------------------------------------------------------------------------------------- | -------- |
| SRCH-1  | Users can search for other users by username or display name.                                 | P1       |
| SRCH-2  | Users can search messages within a specific conversation by keyword.                          | P2       |
| SRCH-3  | Users can perform a global message search across all their conversations.                     | P2       |

---

## 5. Non-Functional Requirements

### 5.1 Performance

| ID      | Requirement                                                                                              |
| ------- | -------------------------------------------------------------------------------------------------------- |
| PERF-1  | Message delivery latency from send to receive must be under 200ms for online recipients.                |
| PERF-2  | Loading the conversation list and the most recent 50 messages must complete within 500ms.               |
| PERF-3  | The system should support at least 500 concurrent WebSocket connections on a single server instance.    |
| PERF-4  | Redis cache hit rate for recent messages should exceed 90% under normal usage patterns.                 |
| PERF-5  | Database queries for message history must use cursor-based pagination to maintain constant performance.  |

### 5.2 Scalability

| ID      | Requirement                                                                                                               |
| ------- | ------------------------------------------------------------------------------------------------------------------------- |
| SCAL-1  | SignalR must use Redis as a backplane so that multiple API instances can broadcast to all connected clients.              |
| SCAL-2  | The messages table must be designed with partitioning in mind (by conversation_id or created_at) for future growth.       |
| SCAL-3  | Stateless API design — no server-side session affinity required beyond WebSocket connections.                              |

### 5.3 Reliability

| ID     | Requirement                                                                                                                  |
| ------ | ---------------------------------------------------------------------------------------------------------------------------- |
| REL-1  | Messages must be persisted to PostgreSQL before delivery confirmation is sent to the sender.                                |
| REL-2  | If a WebSocket connection drops, the client must automatically reconnect and fetch any missed messages since last received.  |
| REL-3  | Redis cache is treated as ephemeral. Loss of cache results in degraded performance but not data loss.                       |

### 5.4 Security

| ID     | Requirement                                                                                                    |
| ------ | -------------------------------------------------------------------------------------------------------------- |
| SEC-1  | All API endpoints (REST and SignalR) require a valid JWT token except registration and login.                  |
| SEC-2  | Users can only send messages to and read messages from conversations they are a member of.                     |
| SEC-3  | SignalR hub methods validate the caller's membership in the target conversation before processing.             |
| SEC-4  | Rate limiting is applied to message sending (e.g., max 30 messages per minute per user).                      |
| SEC-5  | Input is sanitized to prevent XSS in message content rendered on the frontend.                                |
| SEC-6  | CORS is configured to allow only the frontend origin.                                                          |

### 5.5 Observability

| ID     | Requirement                                                                                                  |
| ------ | ------------------------------------------------------------------------------------------------------------ |
| OBS-1  | Structured logging is implemented across all layers using Serilog or a similar library.                      |
| OBS-2  | Health check endpoints are exposed for the API, PostgreSQL, Redis, and RabbitMQ.                             |
| OBS-3  | Key metrics such as active connections, messages per second, and cache hit rate are trackable.                |

---

## 6. Technical Architecture

### 6.1 Technology Stack

| Component         | Technology                                       |
| ----------------- | ------------------------------------------------ |
| Backend Framework | .NET 10 (ASP.NET Core)                           |
| Real-Time         | SignalR (WebSocket with fallback)                |
| Database          | PostgreSQL 16+                                   |
| ORM               | Entity Framework Core                            |
| Cache / Presence  | Redis 7+                                         |
| Message Queue     | RabbitMQ (via MassTransit) — added when needed   |
| Frontend          | React (TypeScript)                               |
| Auth              | JWT (access + refresh tokens)                    |
| File Storage      | MinIO (local development) / AWS S3 (production)  |
| Containerization  | Docker and Docker Compose                        |
| Architecture      | Clean Architecture with CQRS (MediatR)           |

### 6.2 Backend Project Structure

```
src/
├── ChatApp.API                  → Presentation layer (controllers, SignalR hubs, middleware)
├── ChatApp.Application          → Use cases, CQRS commands/queries, interfaces, validators
├── ChatApp.Domain               → Entities, value objects, domain events, enums
├── ChatApp.Infrastructure       → EF Core, Redis, RabbitMQ, JWT, external service implementations

tests/
├── ChatApp.UnitTests
├── ChatApp.IntegrationTests
└── ChatApp.ArchitectureTests
```

Dependency rule: **Domain ← Application ← Infrastructure ← API**. Inner layers never reference outer layers. The Application layer defines interfaces; the Infrastructure layer implements them.

### 6.3 Database Schema Overview

The relational model consists of five core tables.

- **users** — Stores account credentials, profile information, and soft-delete state.
- **conversations** — Represents a chat context. The `type` column distinguishes between `direct`, `group`, and `channel`.
- **conversation_members** — Join table resolving the many-to-many relationship between users and conversations. Contains the member's role, join timestamp, `last_read_at` (for unread calculation), and notification preferences.
- **messages** — Stores message content with foreign keys to conversation and sender. Includes a `metadata` JSONB column for flexible data such as reactions, link previews, edit history, and attachment references. The primary query index is `(conversation_id, created_at DESC)`.
- **attachments** — Tracks uploaded files associated with messages, storing the file URL, size, and content type.

### 6.4 Redis Cache Strategy

| Data                  | Key Pattern                            | Structure   | TTL              | Purpose                                         |
| --------------------- | -------------------------------------- | ----------- | ---------------- | ------------------------------------------------ |
| Online presence       | `presence:{userId}`                    | Hash        | 60s (refreshed)  | Track online/offline status and last seen        |
| Recent messages       | `chat:{conversationId}:messages`       | Sorted Set  | 24 hours         | Cache last 100 messages per conversation         |
| Unread counts         | `unread:{userId}:{conversationId}`     | String/Int  | None             | Atomic increment/reset for unread badge          |
| Typing indicators     | `typing:{conversationId}:{userId}`     | String      | 4s               | Auto-expiring typing state                       |
| Active sessions       | `session:{userId}`                     | Set         | Token TTL        | Track active refresh tokens for invalidation     |
| SignalR connection map| `connections:{userId}`                 | Set         | None (managed)   | Map user IDs to SignalR connection IDs           |

### 6.5 Real-Time Communication Flow

1. Client establishes a SignalR WebSocket connection upon login, authenticated via JWT.
2. The server registers the connection ID against the user ID in Redis and updates the user's presence to "online".
3. When a user sends a message, the SignalR hub handler persists the message to PostgreSQL, pushes it to the Redis message cache, and broadcasts it to the SignalR group mapped to the conversation ID.
4. Typing indicators and presence changes flow exclusively through SignalR and Redis without touching the database.
5. On disconnection, a grace period timer is set. If the user does not reconnect within the grace period, their presence flips to "offline" and a `last_seen` timestamp is written.
6. On reconnection, the client sends the ID of its last received message and the server returns any messages sent after that point to fill the gap.

### 6.6 Message Queue (RabbitMQ) — Deferred Addition

RabbitMQ is intentionally excluded from the initial build. SignalR handles real-time broadcast directly. RabbitMQ should be introduced when any of the following requirements become relevant.

- Push notification delivery to offline users via an external service (e.g., Firebase Cloud Messaging).
- Asynchronous message indexing for full-text search.
- Fan-out to very large groups (200+ members) where synchronous broadcast creates noticeable latency.
- Audit logging or analytics event publishing.

When introduced, the pattern is: the SignalR hub publishes a domain event (e.g., `MessageSentEvent`) to RabbitMQ after the synchronous path (persist + broadcast) completes. Background consumers then handle the asynchronous side effects independently.

---

## 7. Frontend Overview

The React frontend is a single-page application built with TypeScript. It connects to the backend via both REST API calls (for authentication, conversation management, and message history) and a persistent SignalR WebSocket connection (for real-time message delivery, presence, and typing indicators).

### 7.1 Core Views

| View                 | Description                                                                             |
| -------------------- | --------------------------------------------------------------------------------------- |
| Login / Register     | Authentication forms. On success, stores JWT tokens and establishes SignalR connection.  |
| Conversation List    | Sidebar displaying all conversations sorted by recent activity with unread badges.      |
| Chat View            | Main panel showing message history, message input, and typing indicators.               |
| Conversation Settings| Panel for managing group name, members, roles, and notification preferences.            |
| User Profile         | View and edit display name, avatar, and status message.                                 |

### 7.2 State Management

Global state (authentication, active conversation, presence map) is managed via React Context or a lightweight state library such as Zustand. The SignalR connection is initialized once at the application root and event handlers dispatch updates to the relevant state slices.

---

## 8. Development Milestones

### Phase 1 — Foundation (Weeks 1–3)

- Set up the .NET solution with Clean Architecture project structure.
- Configure Docker Compose with PostgreSQL and Redis.
- Implement user registration, login, and JWT authentication.
- Create the database schema and EF Core configurations with initial migration.
- Build the basic React shell with routing and authentication flow.

### Phase 2 — Core Messaging (Weeks 4–6)

- Implement conversation creation (direct and group).
- Build the SignalR ChatHub for real-time message send and receive.
- Implement message persistence and cursor-based pagination for history.
- Build the Redis caching layer for recent messages.
- Build the React chat view with message list, input, and real-time updates.

### Phase 3 — Presence and Notifications (Weeks 7–8)

- Implement online/offline presence tracking via SignalR + Redis.
- Build typing indicators with auto-expiry.
- Implement unread count tracking (Redis counters + `last_read_at` sync).
- Add the conversation list sidebar with unread badges and message previews.

### Phase 4 — Enhanced Features (Weeks 9–11)

- Add message editing and soft deletion.
- Implement reply-to-message threading.
- Add emoji reactions stored in the JSONB metadata column.
- Implement file upload and sharing via MinIO/S3.
- Build conversation management (add/remove members, roles, mute).

### Phase 5 — Polish and Observability (Weeks 12–13)

- Add structured logging with Serilog.
- Implement health check endpoints.
- Add rate limiting to message endpoints.
- Write unit and integration tests for critical paths.
- Add architecture tests to enforce layer dependency rules.
- Implement reconnection logic and missed message recovery on the client.

### Phase 6 — Optional Extensions (Ongoing)

- Introduce RabbitMQ for push notifications and async processing.
- Add full-text message search (PostgreSQL `tsvector` or Elasticsearch).
- Add browser push notifications.
- Implement channel discovery and join flows.

---

## 9. Open Questions

| #  | Question                                                                                          | Status |
| -- | ------------------------------------------------------------------------------------------------- | ------ |
| 1  | Should message search use PostgreSQL full-text search or a dedicated engine like Elasticsearch?   | Open   |
| 2  | What is the maximum supported group size before RabbitMQ fan-out becomes necessary?               | Open   |
| 3  | Should the application support multiple devices per user with synchronized read state?            | Open   |
| 4  | Is there a need for message retention policies (auto-delete messages older than N days)?          | Open   |
| 5  | Should channels support public discoverability or remain invite-only?                             | Open   |

---

## 10. Appendix

### A. Glossary

| Term               | Definition                                                                                     |
| ------------------ | ---------------------------------------------------------------------------------------------- |
| Direct conversation| A private chat between exactly two users.                                                     |
| Group conversation | A chat with three or more invited members, not publicly discoverable.                         |
| Channel            | A conversation that can be discovered and joined by any user.                                 |
| Presence           | The real-time online/offline/typing state of a user, tracked in Redis.                        |
| Backplane          | A Redis Pub/Sub layer that allows multiple SignalR server instances to share broadcast events. |
| Cursor pagination  | Pagination using the last item's identifier or timestamp instead of page numbers.              |
| Soft delete        | Marking a record as deleted without physically removing it from the database.                 |
| CQRS               | Command Query Responsibility Segregation — separating write operations from read operations.  |
