const hostUrl = 'https://api.giphy.com/v1/gifs/search'

const search = async (options) => {
  const params = URLSearchParams(options)

  return await fetch(`{hosturl}${params.toString()}`
  )
}

export default {
  search
}
