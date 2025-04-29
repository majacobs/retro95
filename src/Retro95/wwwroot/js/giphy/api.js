const api_key = '8EGC77F8feUeiOu07LxwWOBa2tfPpasO'
const baseUri = 'https://api.giphy.com/v1/gifs/search'

const search = async (options) => {
  const response = await fetch(`${baseUri}?${new URLSearchParams({ ...options, api_key }).toString()}`)
  const { data, pagination: { total_count } } = await response.json()

  return {
    images: data.map(({ images: { original, fixed_height_downsampled: wide, fixed_width_downsampled: tall }, title }) => ({
      original: `<img src="${original.webp}" alt="${title}" />`,
      downsampled: `<img src="${parseInt(wide.size) > parseInt(tall.size) ? wide.webp : tall.webp}" alt="${title}" />`
    })),
    total_count
  }
}

export default {
  search
}
