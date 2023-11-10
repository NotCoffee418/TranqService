"use client";
import Link from "next/link";
import React from "react";
import { NavLink } from "./NavLink";

const TopNav = () => {
  return (
    <>
      <ul className="flex flex-row space-x-12 justify-center text-xl my-2">
        <NavLink href="/">History</NavLink>
        <NavLink href="/settings">Settings</NavLink>
      </ul>
      <hr />
    </>
  );
};

export default TopNav;
