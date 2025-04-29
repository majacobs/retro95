const api_key = '8EGC77F8feUeiOu07LxwWOBa2tfPpasO'
const hostUrl = 'https://api.giphy.com/v1/gifs/search'

const search = async (options) => {
  const response = await fetch(`${hostUrl}?${new URLSearchParams({ ...options, api_key }).toString()}`)
  const { data } = await response.json()

  return data.map(({ images: { original, fixed_height_downsampled: wide, fixed_width_downsampled: tall }, title }) => ({
    original: `<img src="${original.webp}" alt="${title}" />`,
    downsampled: `<img src="${parseInt(wide.size) > parseInt(tall.size) ? wide.webp : tall.webp}" alt="${title}" />`
  }))
}

export default {
  search
}
