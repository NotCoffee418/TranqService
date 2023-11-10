"use client";
import "./globals.css";
import TopNav from "./_components/menus/TopNav";
import BottomStatus from "./_components/menus/BottomStatus";
import "@fontsource/roboto";

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className="text-white bg-gray-900 flex flex-col h-screen m-0 font-sans">
        <nav>
          <TopNav />
        </nav>

        <main className="flex-grow p-4">{children}</main>

        <div>
          <BottomStatus />
        </div>
      </body>
    </html>
  );
}
