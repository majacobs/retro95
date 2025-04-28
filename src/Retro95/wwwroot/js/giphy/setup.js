import api from './api.js'

const api_key = '8EGC77F8feUeiOu07LxwWOBa2tfPpasO'
const q = ''
const limit = 25
const offset = 0
const rating = 'g'
const lang = 'en'
const bundle = 'messaging_non_clips'

// constructor
function Giphy() {
  const defaults = {
    api_key,
    q,
    limit,
    offset,
    rating
  }

  const mergeWithDefaults = (options = {}) => {
    const params = {
      ...defaults,
      ...options
    }
  }

  this.doSearch = async (q, options) => {
    const response = await api.search(mergeWithDefaults({ ...options, q }))

    return
  }
}

export default Giphy
export { Giphy }
