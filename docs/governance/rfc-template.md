# RFC template

Copy this file to `docs/rfcs/YYYY-MM-DD-topic-slug.md` and fill every section (write the N/A sentence if §6 doesn't apply).
Sections 1–5 are mandatory; section 6 is required unless the RFC is pure
policy (then write "Not applicable — pure policy." and why).

Process: `docs/governance/README.md`. Decision rule: the maintainer accepts or rejects;
acceptance requires section 4 to pass and section 5 to be non-empty.

## 1. Title, date, author, status

Status is `proposed` on submission. Later: `accepted`, `rejected`, or
`superseded` (naming the RFC that supersedes it).

## 2. Context

What exists today, with file and section links. No proposal here — just the
ground the proposal stands on.

## 3. Proposal

What changes, concretely: files, formats, commands, behaviors. Detailed
enough that someone else could implement it without asking you.

## 4. Invariant-filter check

One paragraph each, or an explicit "not affected because…":

- Determinism: same bytes in, same bytes out — preserved how?
- Share-safety: a file from a stranger stays safe to load and run — why?
- Auditability: what ran, on what, with what parameters — where recorded?

A proposal weakening any of the three is closed, not debated
(recipe standard §2).

## 5. Alternatives considered

At least one rejected alternative with its reason. A proposal with no
considered alternative is returned, not reviewed.

## 6. Rollout (required unless pure policy)

Migration, version-bump consequences per `docs/governance/versioning.md`, corpus impact if
any. Pure-policy RFCs: "Not applicable — pure policy." plus one sentence why.
