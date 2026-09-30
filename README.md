# Basic hobby project - Parts Assistant with Applied AI concepts
A little AI-powered parts lookup bot for a made-up auto parts store, built in C#/.NET. Not a real product. just me learning how to actually build and evaluate LLM features properly instead of just vibing with a prompt and hoping for the best.

## What even is this
Built an actual eval dataset before writing the AI part (yes, on purpose)
Made the assistant talk to OpenAI and spit out clean structured JSON, not just vibes-based text
Added a safety net that double-checks the AI isn't making stuff up
Wired everything up to Langfuse so I can actually see what's happening (cost, traces, scores, all of it)
Even got a second AI to grade the first AI's homework 💀

It's not production code. It's a synthetic little sandbox to learn in.

The gist of how it's put together
- Domain — the boring-but-necessary stuff (parts, vehicles, what fits what)
- Assistant — the contract for "an assistant answers questions" + a guardrail that catches hallucinated parts
- Evaluation — the test questions + the scorer that checks if the AI actually got it right
- OpenAi — the real assistant, talking to OpenAI
- Langfuse — sends traces/costs/scores over so I can see it all in one place
- MicrosoftEval — the "AI judges the AI" bit
- tools/ — the actual runnable programs that tie it all together

If you do want to run, you'll need to setup some OPEN AI keys and langfuse key credentials :D

<img width="1895" height="1072" alt="image" src="https://github.com/user-attachments/assets/15deb69f-2c70-4593-b23d-4392cc38b2e1" />
