import React from 'react'

function ErrorMsg({error}) {
  return (
    <p className="text-red-500 text-sm">{error}</p>
  )
}

export default ErrorMsg