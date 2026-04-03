<template>
  <div>
    <h2 class="text-xl font-semibold text-slate-800">Dashboard</h2>
    <p class="mt-1 text-sm text-slate-500">
      Overview of follow-up task status and progress.
    </p>

    <div class="mt-6 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <div class="rounded-lg border border-slate-200 bg-white p-5">
        <p class="text-sm font-medium text-slate-500">Total Tasks</p>
        <p class="mt-1 text-2xl font-semibold text-slate-800">&mdash;</p>
      </div>
      <div class="rounded-lg border border-slate-200 bg-white p-5">
        <p class="text-sm font-medium text-slate-500">Not Started</p>
        <p class="mt-1 text-2xl font-semibold text-slate-800">&mdash;</p>
      </div>
      <div class="rounded-lg border border-slate-200 bg-white p-5">
        <p class="text-sm font-medium text-slate-500">In Progress</p>
        <p class="mt-1 text-2xl font-semibold text-slate-800">&mdash;</p>
      </div>
      <div class="rounded-lg border border-slate-200 bg-white p-5">
        <p class="text-sm font-medium text-slate-500">Completed</p>
        <p class="mt-1 text-2xl font-semibold text-slate-800">&mdash;</p>
      </div>
    </div>
  </div>
</template>
