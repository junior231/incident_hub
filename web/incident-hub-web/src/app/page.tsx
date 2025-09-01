import Link from "next/link";

export default function HomePage() {
  return (
    <main className="flex flex-col items-center justify-center min-h-screen bg-gray-50 dark:bg-gray-900 px-6">
      <div className="text-center max-w-2xl">
        <h1 className="text-4xl sm:text-5xl font-bold text-gray-900 dark:text-white mb-6">
          Welcome to <span className="text-indigo-600">Incident Hub</span>
        </h1>
        <p className="text-lg sm:text-xl text-gray-600 dark:text-gray-300 mb-8">
          Track, manage, and resolve incidents in real time. Stay on top of what
          matters most with a clear, simple workflow.
        </p>

        <div className="flex flex-col sm:flex-row gap-4 justify-center">
          <Link
            href="/incidents"
            className="px-6 py-3 text-lg font-medium rounded-lg text-white bg-indigo-600 hover:bg-indigo-700 transition"
          >
            View Incidents
          </Link>
          <Link
            href="/incidents/new"
            className="px-6 py-3 text-lg font-medium rounded-lg text-indigo-600 border border-indigo-600 hover:bg-indigo-50 dark:hover:bg-gray-800 transition"
          >
            Create Incident
          </Link>
        </div>
      </div>
    </main>
  );
}
