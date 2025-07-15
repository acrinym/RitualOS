# Client CRM Schema

```json
{
  "id": "string",
  "name": "string",
  "role": "string",
  "email": "string",
  "phone": "string",
  "notes": "string",
  "ritual_history": [
    { "id": "ritual_id" }
  ],
  "tags": ["string"],
  "energy_notes": "string",
  "created_at": "yyyy-mm-dd",
  "last_updated": "yyyy-mm-dd"
}
```

Client records are stored as individual JSON files. They can be looked up by `id` and linked to
rituals using the `ClientDatabase` service which manages persistence and beneficiary assignment.
