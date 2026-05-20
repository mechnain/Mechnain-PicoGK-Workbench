import { chromium } from "playwright";
import path from "path";
import { fileURLToPath } from "url";

const base = process.env.WB_BASE_URL || "http://127.0.0.1:5027";
const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const outDir = path.join(root, "docs", "assets", "screenshots");

const pages = [
  ["home.png", "/"],
  ["generator-library.png", "/generators"],
  ["generator-detail.png", "/generators/primitive-test"],
  ["recent-runs.png", "/recent-runs"],
  ["exports.png", "/exports"],
];

const browser = await chromium.launch();
const page = await browser.newPage({ viewport: { width: 1600, height: 900 } });

for (const [file, route] of pages) {
  const url = `${base}${route}`;
  console.log(`Capturing ${file} <- ${url}`);
  await page.goto(url, { waitUntil: "networkidle", timeout: 60000 });
  await page.waitForTimeout(2500);
  await page.screenshot({
    path: path.join(outDir, file),
    fullPage: true,
  });
}

await browser.close();
console.log("Done.");
