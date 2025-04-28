const hostUrl = 'https://api.giphy.com/v1/gifs/search'

const search = async (options) => {
  const { data } = await fetch(`${hostUrl}${URLSearchParams(options).toString()}`)

  return data.map(({ images: { original, fixed_height_downsampled: wide, fixed_width_downsampled: tall } }) => ({
    original: original.webp,
    downsampled: parseInt(wide.size) > parseInt(tall.size) ? wide.webp : tall.webp
  }))
}

export default {
  search
}
