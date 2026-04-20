# Agent Pipeline Design

**Date:** 2026-04-20
**Status:** Approved

## Overview

A contract-based, two-agent pipeline within Claude Code (`.claude/agents/`). The planner produces a strict JSON implementation plan; the coder executes it in dependency order and reports structured results per task. All inter-agent communication is machine-readable JSON — no free-form text. Determinism > creativity.

---

## Section 1: Agent Definitions

### `planner.md` — `claude-opus-4-7`

**Tools:** `Read`, `Glob`, `Grep`, `WebSearch`, `WebFetch`
**Permissions:** Read-only (no file modifications)

**Role:**
- Receive a feature request from the user
- Explore the codebase to discover real files and modules
- Produce a strictly structured implementation plan in JSON
- Must NOT write code or modify files

**Rules:**
- Output MUST be valid JSON — no markdown, no explanations outside the JSON object
- Must follow the Plan Schema exactly (see Section 2)
- Must define task dependencies explicitly
- Must include acceptance criteria for each task
- Must reference real files/modules discovered during exploration
- Tasks must be granular — no large vague tasks
- `files` must be explicit — no "various files"

---

### `coder.md` — `claude-sonnet-4-6`

**Tools:** All tools
**Permissions:** Full access

**Role:**
- Receive a plan in the defined JSON schema
- Build a dependency graph (DAG) from task `depends_on` fields
- Execute tasks in correct dependency order
- Modify the codebase accordingly
- Report a structured JSON result for each completed task

**Rules:**
- MUST NOT change the plan
- MUST execute only what is defined in the tasks
- MUST respect declared dependencies — do not execute a task before its dependencies are done
- MUST produce a structured JSON result for each task (see Section 3)

---

## Section 2: Plan Schema (Strict Contract)

Planner output MUST be valid JSON conforming to this schema:

```json
{
  "feature": "string",
  "overview": "string",
  "tasks": [
    {
      "id": 1,
      "name": "string",
      "layer": "Core | Application | Infrastructure | Presentation",
      "files": ["relative/path/to/file.cs"],
      "action": "create | modify | delete",
      "depends_on": [],
      "description": "clear, implementation-focused instructions",
      "acceptance_criteria": [
        "measurable condition 1",
        "measurable condition 2"
      ]
    }
  ]
}
```

**Constraints:**
- `id` must be unique across all tasks
- `depends_on` must reference valid task IDs only
- No circular dependencies allowed
- Tasks must be granular — one clear responsibility per task
- `files` must list explicit paths — never "various files" or similar

---

## Section 3: Coder Output Schema

For each task, coder MUST return:

```json
{
  "task_id": 1,
  "status": "done | failed",
  "changes": [
    {
      "file": "relative/path",
      "summary": "what was changed"
    }
  ],
  "notes": "optional details or errors"
}
```

---

## Section 4: Tester Output Schema (Future Agent)

Reserved for a future `tester.md` agent. Schema:

```json
{
  "task_id": 1,
  "status": "passed | failed",
  "evidence": "logs, test results, or reasoning",
  "issues": ["optional list of problems"]
}
```

---

## Section 5: Workflow

1. User sends a feature request to the **Planner**
2. Planner explores the codebase, then outputs a JSON plan (strict schema)
3. User performs basic validation:
   - Valid JSON
   - No missing required fields
   - Valid dependency graph (no cycles, no missing IDs)
4. User passes the validated plan to the **Coder**
5. Coder builds a DAG from `depends_on` fields
6. Coder executes tasks in correct order, producing a JSON result per task
7. (Future) Tester validates each task against its `acceptance_criteria`

---

## Key Principles

- This is not chat — this is a contract-based pipeline
- **Planner** = producer
- **Coder** = executor
- **Tester** = validator (future)
- All inter-agent communication = machine-readable JSON
- No free-form text between agents
- Determinism > creativity
