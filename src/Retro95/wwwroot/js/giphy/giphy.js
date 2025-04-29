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
  let currentIndex = 0
  let images = []
  let useDownsampled = false

  const mergeWithDefaults = (options = {}) => ({
    ...defaults,
    ...options
  })

  this.doSearch = async (q, options) => {
    const response = await api.search(mergeWithDefaults({ ...options, q }))
    images = images.concat(response)
  }

  this.render = (mountId, uid) => {
    const wrapper = document.createElement('div')
    wrapper.setAttribute('id', `giphy-comment-${uid}`)
    wrapper.setAttribute('class', 'comment')

    const content = document.createElement('div')
    const { original, downsampled } = images[currentIndex]
    content.innerHTML = useDownsampled ? downsampled : original

    wrapper.appendChild(content)
    document.getElementById(mountId).appendChild(wrapper)
  }

  this.next = () => {
    currentIndex++
    rerender()
  }

  this.prev = () => {
    currentIndex--
    rerender()
  }

  // bitwise XOR
  this.toggleDownsample = () => useDownsampled ^= 1
}

export default Giphy
export {
  Giphy,
  command as giphyCommand
}
