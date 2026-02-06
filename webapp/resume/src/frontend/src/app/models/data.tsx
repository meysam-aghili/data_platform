import GithubIcon from '../shared/components/icons/GithubIcon';
import LinkedInIcon from '../shared/components/icons/LinkedInIcon';
import aboutBackgroundImage from '../statics/images/header-background.webp';
import aboutImage from '../statics/images/header.jpg';
import { About, PortfolioItem, ResumeHistoryItem, SkillGroup, SkillGroupV2, Social } from './interfaces';
import { AppWindow, Calendar, Download, Mail, MapPin, Phone } from "lucide-react";

import githubImage from '../statics/images/portfolio/github.jpg';
// import airflowImage from '../statics/images/portfolio/airflow.png';

export const SectionId = {
  About: 'About',
  Portfolio: 'Portfolio',
  Resume: 'Resume'
} as const;
export type SectionId = (typeof SectionId)[keyof typeof SectionId];


export const socialLinks: Social[] = [
  { label: 'Github', Icon: GithubIcon, href: 'https://github.com/meysam-aghili' },
  { label: 'LinkedIn', Icon: LinkedInIcon, href: 'https://www.linkedin.com/in/meysam-aghili' },
];

export const AboutData: About = {
  imageSrc: aboutImage,
  backImageSrc: aboutBackgroundImage,
  name: `I'm Meysam Aghili`,
  description: (
    <>
      <p className="prose-sm text-start text-stone-200 sm:prose-base lg:prose-lg">
        I'm a <strong className="text-stone-100">Data / Platform Engineer</strong>,
        dedicated and results-oriented with 5 years of experience
        designing, implementing, and maintaining robust data pipelines and
        architectures. <br />
        Proficient in using a variety of technologies.
        I possess strong problem-solving and analytical skills, a passion for learning, a
        sense of responsibility, and the ability to collaborate effectively as a team
        player.
      </p>
    </>
  ),
  actions: [
    {
      href: '/assets/MeysamAghiliResume.pdf',
      text: 'Resume',
      primary: true,
      Icon: Download,
    }
  ],
  aboutItems: [
    { label: 'Location', text: 'Tehran, Iran', Icon: MapPin },
    { label: 'Age', text: '25', Icon: Calendar },
    { label: 'Website', text: 'meysamaghili.ir', Icon: AppWindow },
    { label: 'Email', text: 'meysamaghili533@gmail.com', Icon: Mail },
    { label: 'Phone', text: '+98 9332655928', Icon: Phone },
    { label: 'LinkedIn', text: 'www.linkedin.com/in/meysam-aghili', Icon: Mail },
  ],
};

export const experiences: ResumeHistoryItem[] = [
  {
    timeWindow: "2022 - Present",
    location: "Digikala, Inc.",
    title: "Data/Platform Engineer",
    description: (
      <ul className="text-gray-300 list-disc">
        <li className='ml-6 mb-3'>
        
            <span className='font-bold  mb-1'>Kafka & Data Integration Engineering: </span>
            <ul className="text-gray-300 list-disc">
              <li className='ml-6 mb-1'>
                <p>
                  Designed, implemented, and maintained a scalable, version-controlled Kafka cluster supporting real-time data ingestion and processing across multiple environments.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Developed and managed high-throughput data pipelines integrating MySQL, PostgreSQL, and other sources into data lakes and data warehouses.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Performed Kafka upgrades, production maintenance, and version alignment to ensure system stability, security, and performance.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Integrated Prometheus and Grafana for proactive cluster monitoring and alerting, improving system reliability and observability.
                </p>
              </li>
            </ul>
    
        </li>
        <li className='ml-6 mb-3'>
        
            <span className='font-bold mb-1'>.NET Application & API Development: </span>
            <ul className="text-gray-300 list-disc">
              <li className='ml-6 mb-1'>
                <p>
                  Designed and built .NET applications and REST APIs using ASP.NET Web API and Blazor, applying clean architecture principles and enterprise design patterns.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Implemented structured logging, data persistence with Entity Framework (EF), and containerization with Docker.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Developed and published a reusable utilities library on NuGet, enhancing code reusability and project standardization.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Built comprehensive unit and integration test suites to ensure high reliability and maintainability.
                </p>
              </li>
            </ul>
      
        </li>
        <li className='ml-6 mb-3'>
    
            <span className='font-bold mb-1'>Data Catalog & Governance Platform: </span>
            <ul className="text-gray-300 list-disc">
              <li className='ml-6 mb-1'>
                <p>
                  Architected and developed a full-stack Data Catalog application to centralize metadata, visualize data structures, and manage data governance.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Built data classification, tagging, and access management features supporting compliance and security requirements.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Implemented LDAP integration, role-based (RBAC) and attribute-based access control, and PostgreSQL row-level security for fine-grained permissions.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Engineered backend services in .NET, frontend in React, and containerized the stack for production-ready deployments.
                </p>
              </li>
            </ul>
     
        </li>
        <li className='ml-6 mb-3'>
     
            <span className='font-bold mb-1'>Airflow Infrastructure & Workflow Orchestration: </span>
            <ul className="text-gray-300 list-disc">
              <li className='ml-6 mb-1'>
                <p>
                  Designed and maintained a production-grade Apache Airflow cluster for orchestrating BI and data workflows.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Utilized Docker Operator for dependency isolation and created CI/CD pipelines and custom DAG templates to streamline project deployment.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Automated Airflow project onboarding and deployment, improving scalability and consistency across teams.
                </p>
              </li>
            </ul>
    
        </li>
        <li className='ml-6 mb-3'>
    
            <span className='font-bold mb-1'>Monitoring, Observability & Alerting Infrastructure: </span>
            <ul className="text-gray-300 list-disc">
              <li className='ml-6 mb-1'>
                <p>
                  Built an organization-wide monitoring and alerting system with Prometheus and Grafana to track performance of all projects and infrastructure components.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Developed custom exporters for detailed metrics collection from Airflow, Kafka, and application services.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Created a production-grade Alerting Middleware API (.NET) for centralized notification delivery to Slack and Microsoft Teams, supporting dynamic webhooks and templated messages.
                </p>
              </li>
            </ul>
       
        </li>
        <li className='ml-6 mb-3'>
       
            <span className='font-bold mb-1'>Version Control & CI/CD Automation: </span>
            <ul className="text-gray-300 list-disc">
              <li className='ml-6 mb-1'>
                <p>
                  Implemented advanced Git workflows (branching, merge requests, release management) to maintain structured and traceable development cycles.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Built automated CI/CD pipelines using GitLab CI for building, testing, and deploying data and application projects.
                </p>
              </li>
            </ul>
      
        </li>
        <li className='ml-6 mb-3'>
    
            <span className='font-bold mb-1'>Containerization & Orchestration: </span>
            <ul className="text-gray-300 list-disc">
              <li className='ml-6 mb-1'>
                <p>
                  Containerized key applications and services across the organization using Docker for consistency, portability, and scalability.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Leveraged Docker Swarm for orchestrating multi-container deployments and optimizing resource utilization.
                </p>
              </li>
            </ul>
   
        </li>
        <li className='ml-6 mb-3'>
      
            <span className='font-bold mb-1'>Database Design & Management: </span>
            <ul className="text-gray-300 list-disc">
              <li className='ml-6 mb-1'>
                <p>
                  Designed, implemented, and optimized databases including PostgreSQL, SQL Server, MongoDB, ClickHouse, and Redis.
                </p>
              </li>
              <li className='ml-6 mb-1'>
                <p>
                  Skilled in data modeling, indexing, query optimization, replication, and backup strategies to ensure performance, scalability, and reliability.
                </p>
              </li>
            </ul>
     
        </li>
      </ul>
    )
  },
  {
    timeWindow: "2021 - 2022",
    location: "Digikala, Inc.",
    title: "Senior BI Developer | Data Analyst",
    description: (
      <ul className="text-gray-300 list-disc">
        <li className='ml-6 mb-3'>
          <p>
            Lead the design, development, and optimization of dashboards, data models, and ETL pipelines.
          </p>
        </li>
        <li className='ml-6 mb-3'>
          <p>
            Partner with business stakeholders to identify KPIs, define reporting requirements, and deliver actionable insights.
          </p>
        </li>
        <li className='ml-6 mb-3'>
          <p>
            Develop automated reporting systems and scalable data solutions to ensure accuracy, accessibility, and consistency across business units.
          </p>
        </li>
        <li className='ml-6 mb-3'>
          <p>
            Perform advanced data analysis to uncover trends, opportunities, and risks that drive strategic initiatives.
          </p>
        </li>
        <li className='ml-6 mb-3'>
          <p>
            Mentor junior analysts and contribute to continuous improvement of BI best practices.
          </p>
        </li>
      </ul>
    )
  },
  {
    timeWindow: "2020 - 2021",
    location: "Nikamooz, Inc.",
    title: "BI Developer",
    description: (
      <ul className="text-gray-300 list-disc">
        <li className='ml-6 mb-3'>
          <p>
            Build and optimize data models, ETL pipelines, and data warehouses to ensure efficient data flow and accuracy and scalability.
          </p>
        </li>
        <li className='ml-6 mb-3'>
          <p>
            Design, develop, and maintain BI dashboards and analytical reports using tools such as Power BI, SSIS.
          </p>
        </li>
        <li className='ml-6 mb-3'>
          <p>
            Collaborate with cross-functional teams to identify data requirements and translate business needs into technical solutions.
          </p>
        </li>
        <li className='ml-6 mb-3'>
          <p>
            Analyze large datasets to uncover trends, patterns, and opportunities for business improvement.
          </p>
        </li>
      </ul>
    )
  }
];

export const educations: ResumeHistoryItem[] = [
  {
    timeWindow: "2018 - 2022",
    location: "University of Science and Research",
    title: "Bachelor of Computer Science",
    description: (
      <ul className="text-gray-300 list-disc">
        <li className='ml-6 mb-3'>
          <p>
            Completed a Bachelor of Computer Science with a focus on software development, algorithms, and data structures.<br />
            Gained hands-on experience in programming languages such as C#, Python, and JavaScript, as well as application development.<br />
            Participated in collaborative projects, coding competitions, and research initiatives, building a strong foundation in problem-solving, system design, and modern software engineering practices.
          </p>
        </li>
      </ul>
    )
  }
];

export const skillItems: SkillGroup[] = [
  {
    title: 'Frontend',
    items: [
      { title: 'React', level: 8 },
      { title: 'TypeScript', level: 7 },
      { title: 'GraphQL', level: 5 },
      { title: 'Vue', level: 4 },
    ],
  },
  {
    title: 'Mobile',
    items: [
      { title: 'React Native', level: 7 },
      { title: 'Java', level: 5 },
      { title: 'Flutter', level: 3 },
    ],
  },
  {
    title: 'Backend',
    items: [
      { title: 'Node.js', level: 8 },
      { title: 'Rust', level: 5 },
      { title: 'Go', level: 6 },
      { title: 'Python', level: 4 },
    ],
  },
  {
    title: 'Security',
    items: [
      { title: 'Application security', level: 6 },
      { title: 'Network security', level: 5 },
      { title: 'Data compliance', level: 4 },
    ],
  },
];

export const skillItemsV2: SkillGroupV2 = {
  experiencedIn: ["Apache Kafka", ".NET", "Python", "Docker Swarm", "React, Nextjs", "GitLab-CI", "SQLServer, Postgres", "SSIS, SSAS", "Debezium", "Airflow", "PowerBI", "ML"],
  experimentedWith: ["ClickHouse", "Grafana", "Prometheus, InfluxDB", "MongoDB, Redis, Elastic", "ksqlDB", "MinIO", "FastAPI", "RabbitMQ"],
  interestedIn: ["Data", "Engineering", "DevOps", "IOT"],
};

export const portfolioItems: PortfolioItem[] = [
  {
    title: 'Projects Code',
    description: 'Projects Code Repo.',
    url: 'https://github.com/meysam-aghili/resume',
    image: githubImage,
  },
  // {
  //   title: 'Airflow',
  //   description: 'Data workflows tool.',
  //   url: '',
  //   image: airflowImage,
  // },
];
