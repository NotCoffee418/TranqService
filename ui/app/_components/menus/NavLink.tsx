"use client";
import React from "react";
import Link, { LinkProps } from "next/link";
import { useRouter } from "next/router";
import { usePathname } from "next/navigation";

export interface NavLinkProps extends LinkProps {
  children: React.ReactElement | string;
}

export function NavLink({ children, href }: NavLinkProps) {
  let dynClass = "";
  if (usePathname() === href) {
    dynClass += "underline";
  }
  return (
    <li>
      <Link href={href} className={dynClass}>
        {children}
      </Link>
    </li>
  );
}
