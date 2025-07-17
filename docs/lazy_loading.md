# Async Document Loading

RitualOS now loads document content on a background thread to keep the UI snappy.
`DocumentLoader.LoadAsync` reads files asynchronously and is used by
`DocumentViewerViewModel`. Large PDFs and EPUBs are processed with `Task.Run` so
loading does not block the main window.
