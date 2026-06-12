# Architecture Notes

## API Layer
Owns HTTP concerns, authentication setup, authorization policies, and request/response mapping.

## Application Layer
Owns use case coordination and depends on abstractions, not infrastructure implementations.

## Domain Layer
Owns core business concepts and rules.

## Infrastructure Layer
Owns external implementation details such as SQL, authentication adapters, and infrastructure package integration.

## Test Layer
Uses Moq for simple collaborator isolation, Awesome Assertions for readable assertions, and in-memory fakes when behavior is more important than interaction verification.