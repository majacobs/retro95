import api from './api.js'

const q = ''
const limit = 5
const offset = 0
const rating = 'pg-13'
const lang = 'en'
const bundle = 'messaging_non_clips'
const command = 'giphy'
// constructor
function Giphy() {
  const defaults = {
    q,
    limit,
    offset,
    rating,
    lang,
    bundle
  }

  const mergeWithDefaults = (options = {}) => ({
    ...defaults,
    ...options
  })

  this.doSearch = async (q, options) => {
    const response = await api.search(mergeWithDefaults({ ...options, q }))

    console.log(response)

    return
  }
}

export default Giphy
export {
  Giphy,
  command as giphyCommand
}
