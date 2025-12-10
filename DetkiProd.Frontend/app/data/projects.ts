export type Project = {
  id: number
  name: string
  date?: string
  year?: string
  tools?: string | string[]
  videoUrl: string
}

export const projects: Project[] = [
  {
    id: 1,
    name: 'Gilmurt',
    date: '2025-01-01',
    year: '2025',
    tools: 'Blender, Nuke, Houdini',
    videoUrl: '/gilmurt_cut_for_web.mp4',
  },
  {
    id: 2,
    name: 'Showreel',
    date: '2024-06-01',
    year: '2024',
    tools: 'After Effects, Premiere Pro',
    videoUrl: '/gilmurt_cut_for_web.mp4',
  },
]

export function getProjectById(id?: string | number) {
  if (id === undefined || id === null) return undefined
  return projects.find((item) => String(item.id) === String(id))
}

