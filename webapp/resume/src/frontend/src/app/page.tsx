import Image from "next/image";
import Header from "./components/Header";
import About from "./components/About";
import Portfolio from "./components/Portfolio";
import Footer from "./components/Footer";
import Resume from "./components/Resume";

export default function Home() {
  return (
    <>
      <Header />
      <About />
      <Resume />
      <Portfolio />
      <Footer />
    </>
  );
}
