import api from './api.js'

const command = 'giphy'
const defaults = {
  limit: 25,
  offset: 0,
  rating: 'pg-13',
  lang: 'en',
  bundle: 'messaging_non_clips'
}

// constructor
function Giphy(downsample = 0) {
  let currentIndex = 0
  let total = 0
  let images = []
  let useDownsampled = downsample
  let id = ''
  let query = ''

  const getImage = () => {
    const { original, downsampled } = images[currentIndex]

    return useDownsampled ? downsampled : original
  }

  const mergeWithDefaults = (options = {}) => ({
    ...defaults,
    ...options
  })

  const next = async () => {
    currentIndex++

    if (currentIndex >= images.length && currentIndex <= total) await this.doSearch(query, mergeWithDefaults({ offset: images.length }))

    rerender()
  }

  const previous = () => {
    if (currentIndex === 0) return

    currentIndex--

    rerender()
  }

  const rerender = () => {
    document.getElementById(`${id}-image`).innerHTML = getImage()
  }

  const submit = () => {
    const column = document.getElementById(id).parentElement.parentElement

    column.querySelector('textarea').value = getImage()
    column.querySelector('input[id$="-render-as"]').value = 'Giphy'

    // Remove comments are also submit buttons, we want the very last one which is our post comment
    const nodes = column.querySelectorAll('button[type="submit"]')
    nodes[nodes.length - 1].click()
  }

  this.doSearch = async (q, options) => {
    query = q
    const { images: response, total_count } = await api.search(mergeWithDefaults({ ...options, q }))
    images = images.concat(response)
    total = total_count
  }

  this.render = (mountId, uid) => {
    id = `giphy-comment-${uid}`

    const wrapper = document.createElement('div')
    wrapper.setAttribute('id', id)
    wrapper.setAttribute('class', 'comment')

    const content = document.createElement('div')
    content.setAttribute('id', `${id}-image`)
    content.innerHTML = getImage()

    const buttonWrapper = document.createElement('div')
    const previousButton = document.createElement('button')
    const nextButton = document.createElement('button')
    const submitButton = document.createElement('button')

    previousButton.setAttribute('class', 'wide')
    previousButton.innerText = 'Previous'
    previousButton.addEventListener('click', previous)

    nextButton.setAttribute('class', 'wide')
    nextButton.innerText = 'Next'
    nextButton.addEventListener('click', next)

    submitButton.setAttribute('class', 'wide')
    submitButton.innerText = 'Submit'
    submitButton.addEventListener('click', submit)

    buttonWrapper.appendChild(previousButton)
    buttonWrapper.appendChild(nextButton)
    buttonWrapper.appendChild(submitButton)

    wrapper.appendChild(content)
    wrapper.appendChild(buttonWrapper)

    document.getElementById(mountId).appendChild(wrapper)
  }

  // bitwise XOR
  this.setDownsample = (enabled) => {
    useDownsampled = enabled

    rerender()
  }
}

export default Giphy
export {
  Giphy,
  command as giphyCommand
}
