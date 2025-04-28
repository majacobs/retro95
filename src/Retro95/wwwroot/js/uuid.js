const __generatedIds = new Set()

export default function generateUUID() {
  while (true) {
    // Simple, nay, borderline primitive collision check
    const [uid] = crypto.randomUUID().split('-')
    if (!__generatedIds.has(uid)) {
      __generatedIds.add(uid)
      return uid
    }
  }
}
