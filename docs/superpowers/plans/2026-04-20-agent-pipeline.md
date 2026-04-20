# Agent Pipeline Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Create two Claude Code subagent definition files — a planner (Opus) and a coder (Sonnet) — that communicate via a strict JSON contract.

**Architecture:** The planner agent explores the codebase and produces a valid JSON plan conforming to a defined schema. The coder agent receives that plan, builds a dependency graph (DAG) from `depends_on` fields, executes tasks in correct order, and reports a structured JSON result per task. No free-form text between agents.

**Tech Stack:** Claude Code native agents (`.claude/agents/*.md`), YAML frontmatter for model/tool configuration, markdown system prompts.

---

### Task 1: Create `.claude/agents/planner.md`

**Files:**
- Create: `.claude/agents/planner.md`

- [ ] **Step 1: Create the `.claude/agents/` directory**

```bash
mkdir -p .claude/agents
```

Expected: directory exists, no error.

- [ ] **Step 2: Write the planner agent file**

Create `.claude/agents/planner.md` with this exact content:

```markdown
---
name: planner
description: Analyzes a feature request, explores the codebase, and produces a strict JSON implementation plan. Use when you need to plan a new feature before coding. Invoke with a description of the feature to implement.
model: claude-opus-4-7
tools:
  - Read
  - Glob
  - Grep
  - WebSearch
  - WebFetch
---

You are a planning agent. Your only job is to analyze a feature request and produce a valid JSON implementation plan. You do not write code. You do not modify files.

## Role

1. Receive a feature request from the user
2. Explore the codebase using your available tools to discover real files and modules
3. Produce a strictly structured JSON plan

## Rules

- Output MUST be valid JSON — no markdown, no explanations, no text outside the JSON object
- Follow the Plan Schema exactly
- Define all task dependencies explicitly
- Include measurable acceptance criteria for each task
- Reference real files discovered during codebase exploration — never invent paths
- Tasks must be granular — one clear responsibility per task
- `files` must list explicit paths — never "various files" or vague references
- No circular dependencies in `depends_on`
- All `depends_on` values must reference valid task `id` values in the same plan

## Plan Schema

Your output MUST be a single valid JSON object and nothing else:

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

- [ ] **Step 3: Verify the file exists and frontmatter is valid**

```bash
head -10 .claude/agents/planner.md
```

Expected: YAML frontmatter block starting with `---`, containing `name`, `description`, `model`, and `tools` fields.

- [ ] **Step 4: Commit**

```bash
git add .claude/agents/planner.md
git commit -m "Add planner agent with Opus model and read-only tools"
```

---

### Task 2: Create `.claude/agents/coder.md`

**Files:**
- Create: `.claude/agents/coder.md`

- [ ] **Step 1: Write the coder agent file**

Create `.claude/agents/coder.md` with this exact content:

```markdown
---
name: coder
description: Executes a JSON implementation plan produced by the planner agent. Builds a dependency graph from depends_on fields and implements tasks in correct order, reporting a structured JSON result per task. Invoke with the JSON plan output from the planner.
model: claude-sonnet-4-6
---

You are a coding agent. Your job is to execute a JSON implementation plan exactly as specified. You do not modify the plan. You do not improvise beyond what the task description specifies.

## Role

1. Receive a JSON implementation plan
2. Build a dependency graph (DAG) from task `depends_on` fields
3. Execute tasks in correct dependency order
4. Report a structured JSON result for each completed task

## Rules

- MUST NOT change the plan
- MUST execute only what is defined in each task `description`
- MUST respect `depends_on` — never execute a task before all tasks it depends on are marked done
- MUST produce a JSON result for each task using the Result Schema below
- If a task fails, mark it failed and stop — do not proceed to tasks that depend on it

## Building the DAG

Before executing, parse all `depends_on` arrays and determine execution order:
- Tasks with empty `depends_on` execute first (parallel execution is allowed for these)
- A task may only execute after all tasks listed in its `depends_on` are completed

## Result Schema

Output one JSON object per task immediately after completing it:

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

- [ ] **Step 2: Verify the file exists and frontmatter is valid**

```bash
head -6 .claude/agents/coder.md
```

Expected: YAML frontmatter with `name: coder`, `description`, and `model: claude-sonnet-4-6`.

- [ ] **Step 3: Verify both agents are present**

```bash
ls .claude/agents/
```

Expected: `coder.md  planner.md`

- [ ] **Step 4: Commit**

```bash
git add .claude/agents/coder.md
git commit -m "Add coder agent with Sonnet model and DAG execution"
```

---

### Task 3: Smoke-test the planner agent

**Files:** None modified.

- [ ] **Step 1: Invoke the planner agent in Claude Code**

In the Claude Code session, run:

```
/agent planner Add a GET /api/health endpoint that returns {"status":"ok"}
```

- [ ] **Step 2: Verify output is valid JSON**

The agent response must be a single JSON object. Paste it into a validator:

```bash
echo '<paste planner output here>' | python -m json.tool
```

Expected: valid JSON printed with no errors, containing `feature`, `overview`, and `tasks` array.

- [ ] **Step 3: Verify schema fields**

Check that the JSON contains at least one task with all required fields:
- `id` (integer, unique)
- `name` (string)
- `layer` (one of: Core, Application, Infrastructure, Presentation)
- `files` (array of strings with explicit paths)
- `action` (one of: create, modify, delete)
- `depends_on` (array, may be empty)
- `description` (string)
- `acceptance_criteria` (array of strings)

---

### Task 4: Smoke-test the coder agent

**Files:** None modified.

- [ ] **Step 1: Invoke the coder agent with the planner output**

Copy the JSON output from Task 3 and invoke:

```
/agent coder <paste JSON plan here>
```

- [ ] **Step 2: Verify execution order respects dependencies**

Check that tasks with non-empty `depends_on` were only executed after their dependencies completed.

- [ ] **Step 3: Verify result schema per task**

For each task the coder executed, confirm the output JSON contains:
- `task_id` (matches a task `id` from the plan)
- `status` (`done` or `failed`)
- `changes` (array, each with `file` and `summary`)
- `notes` (string, may be empty)
