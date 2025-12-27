import React from 'react'
import { Link } from 'react-router-dom';

export default function Header() {
  return (
          <header className="flex items-center justify-between whitespace-nowrap 
      border-b border-solid border-border-color dark:border-[#3a392a] px-6 py-4 lg:px-10 lg:py-5 
      bg-background-light/90 dark:bg-background-dark/90 backdrop-blur-sm sticky top-0 z-50">
        <div className="flex items-center gap-3">
          <div className="size-8 flex items-center justify-center text-primary">

          </div>
          <h2 className="text-xl font-bold tracking-[-0.015em]">
            Bookstore
          </h2>
        </div>

        <div className="hidden sm:flex flex-1 justify-end gap-8">
          <Link className="text-sm font-medium hover:text-primary transition-colors" to="/">
            Home
          </Link>
          <Link className="text-sm font-medium hover:text-primary transition-colors" to="/about">
            About Us
          </Link>
          <Link className="text-sm font-medium hover:text-primary transition-colors" to="/contact">
            Contact
          </Link>
        </div>

        <div className="flex sm:hidden">
          <span className="material-symbols-outlined cursor-pointer">
            menu
          </span>
        </div>
      </header>
  )
}
