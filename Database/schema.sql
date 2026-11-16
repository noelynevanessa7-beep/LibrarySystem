-- Library Book Reservation and Borrowing System (PostgreSQL)
-- Run automatically by the app at start-up; safe to run many times.

CREATE TABLE IF NOT EXISTS users (
    id             SERIAL PRIMARY KEY,
    full_name      VARCHAR(100) NOT NULL,
    member_number  VARCHAR(30)  UNIQUE,              -- student or staff number (members only)
    email          VARCHAR(150) NOT NULL UNIQUE,
    password_hash  TEXT         NOT NULL,
    role           VARCHAR(15)  NOT NULL CHECK (role IN ('member', 'librarian', 'administrator'))
);

CREATE TABLE IF NOT EXISTS books (
    id      SERIAL PRIMARY KEY,
    title   VARCHAR(200) NOT NULL,
    author  VARCHAR(150) NOT NULL,
    section VARCHAR(80)  NOT NULL,
    copies  INT          NOT NULL CHECK (copies >= 0)
);

CREATE TABLE IF NOT EXISTS loans (
    id            SERIAL PRIMARY KEY,
    book_id       INT     NOT NULL REFERENCES books(id) ON DELETE CASCADE,
    member_id     INT     NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    borrow_date   DATE    NOT NULL,
    due_date      DATE    NOT NULL,
    return_date   DATE,                                -- NULL = still borrowed
    renewed       BOOLEAN NOT NULL DEFAULT FALSE,
    reminder_sent BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS reservations (
    id           SERIAL PRIMARY KEY,
    book_id      INT         NOT NULL REFERENCES books(id) ON DELETE CASCADE,
    member_id    INT         NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    created_at   TIMESTAMP   NOT NULL DEFAULT NOW(),
    status       VARCHAR(12) NOT NULL DEFAULT 'waiting'
                 CHECK (status IN ('waiting', 'ready', 'collected', 'cancelled', 'expired')),
    hold_expires DATE                                  -- set when status = 'ready' (kept 2 days)
);

-- A member can have only one active reservation per book (REQ-3.1)
CREATE UNIQUE INDEX IF NOT EXISTS ux_active_reservation
    ON reservations (book_id, member_id) WHERE status IN ('waiting', 'ready');

CREATE TABLE IF NOT EXISTS notifications (
    id         SERIAL PRIMARY KEY,
    member_id  INT       NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    message    TEXT      NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    is_read    BOOLEAN   NOT NULL DEFAULT FALSE
);
